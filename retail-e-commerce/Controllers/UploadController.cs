using Microsoft.AspNetCore.Mvc;
using retail_e_commerce.Base;
using retail_e_commerce.DTOs;

namespace retail_e_commerce.Controllers
{
    public class FileDetailCMS
    {
        public string FileUrl { get; set; }
        public string FileName { get; set; }
        public long Length { get; set; }
        public string FileType { get; set; }
    }
    public class UploadFileResponceCMS
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public List<FileDetailCMS> Data { get; set; }
    }
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICloudflareR2Service _cloudflareR2Service;
        private static List<string> ALLOWED_IMAGE_MIME_TYPES = new List<string> { "image/jpeg", "image/png", "image/gif", "image/webp" };
        public UploadController(IHttpContextAccessor httpContextAccessor, ICloudflareR2Service cloudflareR2Service)
        {
            _httpContextAccessor = httpContextAccessor;
            _cloudflareR2Service = cloudflareR2Service;
        }
        [HttpPost]
        public async Task<UploadFileResponceCMS> Image()
        {
            var result = new UploadFileResponceCMS
            {
                ID = ResultID.ERROR
            };
            var data = new List<FileDetailCMS>();

            var allowedImageMimeTypes = ALLOWED_IMAGE_MIME_TYPES;

            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var listFileData = httpContext?.Request.Form?.Files;

                if (listFileData == null || listFileData.Count == 0)
                    return result;

                foreach (var filedata in listFileData)
                {
                    if (filedata.Length <= 0)
                        continue;

                    // 1) Validate MIME type giống cũ
                    if (!allowedImageMimeTypes.Contains(filedata.ContentType))
                    {
                        return new UploadFileResponceCMS
                        {
                            ID = ResultID.ERROR,
                            Message = "DATA_IS_NOT_ALLOW_UPLOAD"
                        };
                    }

                    // 2) Tạo key/path lưu trên R2
                    var ext = Path.GetExtension(filedata.FileName);
                    if (string.IsNullOrWhiteSpace(ext))
                    {
                        // fallback: nếu không có đuôi
                        ext = ".bin";
                    }

                    var folder = DateTime.UtcNow.ToString("yyyy/MM/dd");
                    var fileName = $"{Guid.NewGuid():N}{ext}";
                    var key = $"uploads/{folder}/{fileName}";
                    // hoặc giữ giống cấu trúc cũ nếu bạn muốn

                    // 3) Upload lên R2
                    await using var stream = filedata.OpenReadStream();

                    // _cloudflareR2Service là service bạn inject qua constructor
                    var publicUrl = await _cloudflareR2Service.UploadAsync(
                        stream,
                        key,
                        filedata.ContentType,
                        httpContext.RequestAborted
                    );

                    // 4) Add vào list data, giữ nguyên model cũ
                    data.Add(new FileDetailCMS
                    {
                        FileUrl = publicUrl,                        // trước là CommonUtils.IMAGE_SERVER_URI + file name
                        FileName = fileName,                        // hoặc Path.GetFileName(key)
                        Length = filedata.Length,
                        FileType = filedata.ContentType
                    });
                }

                if (data.Count > 0)
                {
                    return new UploadFileResponceCMS
                    {
                        ID = ResultID.SUCCESS,
                        Message = "Success",
                        Data = data
                    };
                }
            }
            catch (System.Exception ex)
            {
                //FileLogger.WriteLog(ex.ToString(), $"BTEST{DateTime.Now.ToLongDateString()}");
                //FileLogger.WriteException(ex);
            }

            return result;
        }

    }
}
