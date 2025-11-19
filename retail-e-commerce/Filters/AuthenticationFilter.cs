using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using retail_e_commerce.Common;
using System.Globalization;
using System.Net;

namespace retail_e_commerce.Filters
{
    public class AuthenticationFilter : IAsyncActionFilter, IOrderedFilter
    {
        public int Order => int.MinValue + 100;
        private readonly IConfiguration _configuration;

        private static ILogExtension nnpnLog = LogExtension.InitLogger();
        public AuthenticationFilter(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requestedLanguage = context.HttpContext.Request.Headers["Pn-Client-Language"].ToString();
            if (!string.IsNullOrEmpty(requestedLanguage))
            {
                var culture = new CultureInfo(requestedLanguage);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            Exception? exception = null;
            var reqTime = DateTime.Now;
            ActionExecutedContext? response = null;

            var ipAddress = context.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.HttpContext.Connection?.RemoteIpAddress?.ToString() ?? "N/A";
            }
            try
            {
                var listIP = _configuration["System:BlockIP"]
                    ?.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(ip => ip.Trim())
                    .ToList();
                context.HttpContext.Items["UserIP"] = ipAddress;
                if (listIP?.Exists(l => l == ipAddress) == true)
                {
                    context.Result = new JsonResult(new
                    {
                        Success = false,
                        Message = "Access denied. Your IP has been blocked."
                    })
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden
                    };
                    return;
                }

                response = await next();

                return;
            }
            finally
            {
                nnpnLog.Info($"IP: {ipAddress} | " + FormatLog4NetString(context?.ActionArguments?.Values, reqTime, response?.Result, DateTime.Now));
            }
        }

        private string FormatLog4NetString(object? Request, DateTime RequestTime, object? Respone, DateTime ResopneTime)
        {
            string json = string.Empty;
            string str = string.Empty;
            try
            {
                var newObj = new { Req = Request, ReqTime = RequestTime, Res = Respone, ResTime = ResopneTime, Took = (ResopneTime - RequestTime).Milliseconds + "ms" };
                json = JsonConvert.SerializeObject(newObj, Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    });
            }
            catch (JsonSerializationException e)
            {
                var newObj = new { Req = Request, ReqTime = RequestTime, Res = "", ResTime = ResopneTime, Took = (ResopneTime - RequestTime).Milliseconds + "ms" };
                json = JsonConvert.SerializeObject(newObj, Formatting.None,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    });
            }

            if (json.Length > 500)
            {
                str = json.Remove(249, json.Length - 498).Insert(249, "...");
            }
            else
            {
                str = json;
            }
            return str;
        }
    }
}
