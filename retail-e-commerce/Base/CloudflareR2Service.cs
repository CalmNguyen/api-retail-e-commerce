using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Newtonsoft.Json;

namespace retail_e_commerce.Base
{
    public class CloudflareR2Options
    {
        public string AccountId { get; set; } = "";
        public string AccessKeyId { get; set; } = "";
        public string SecretAccessKey { get; set; } = "";
        public string BucketName { get; set; } = "";
        public string PublicBaseUrl { get; set; } = ""; // cdn.ttsforfree.com
    }

    public interface ICloudflareR2Service
    {
        Task<string> UploadAsync(Stream fileStream, string key, string contentType, CancellationToken ct = default);
    }
    public class CloudflareR2Service : ICloudflareR2Service
    {
        private readonly IMinioClient _minio;
        private readonly CloudflareR2Options _options;

        public CloudflareR2Service(IOptions<CloudflareR2Options> options)
        {
            _options = options.Value;
            Console.WriteLine("DEBUG — CloudflareR2Options:");
            Console.WriteLine("AccountId = " + _options.AccountId);
            Console.WriteLine("BucketName = " + _options.BucketName);
            Console.WriteLine("PublicBaseUrl = " + _options.PublicBaseUrl);

            _minio = new MinioClient()
                .WithEndpoint($"{_options.AccountId}.eu.r2.cloudflarestorage.com")
                .WithSSL(true)
                .WithCredentials(_options.AccessKeyId, _options.SecretAccessKey)
                .WithRegion("auto")
                .Build();
        }
        public async Task<string> UploadAsync(
            Stream fileStream,
            string key,
            string contentType,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(_options.BucketName))
                throw new Exception("CloudflareR2Options.BucketName is null or empty");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("key must not be null or empty", nameof(key));

            if (fileStream == null)
                throw new ArgumentNullException(nameof(fileStream));

            // đảm bảo stream seekable (IFormFile.OpenReadStream nhiều khi không)
            if (!fileStream.CanSeek)
            {
                var buffer = new MemoryStream();
                await fileStream.CopyToAsync(buffer, ct);
                buffer.Position = 0;
                fileStream = buffer;
            }

            var length = fileStream.Length;

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(_options.BucketName)
                .WithObject(key)
                .WithStreamData(fileStream)
                .WithObjectSize(length)
                .WithContentType(string.IsNullOrWhiteSpace(contentType)
                    ? "application/octet-stream"
                    : contentType);

            try
            {
                var t = JsonConvert.SerializeObject(putObjectArgs);

                await _minio.PutObjectAsync(putObjectArgs, ct);
            }
            catch (Exception ex)
            {
                throw;
            }

            return $"{_options.PublicBaseUrl.TrimEnd('/')}/{key}";
        }
    }
}
