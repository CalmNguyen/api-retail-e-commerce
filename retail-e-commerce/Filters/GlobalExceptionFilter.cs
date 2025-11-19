using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using retail_e_commerce.Common;

namespace retail_e_commerce.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public int Order => int.MinValue + 100;
        private static ILogExtension nnpnLog = LogExtension.InitLogger();
        public void OnException(ExceptionContext context)
        {
            context.ExceptionHandled = true;
            nnpnLog.Error(JsonConvert.SerializeObject(context));
        }
    }
}
