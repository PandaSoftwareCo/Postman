using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace PostmanApi.Filters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            Exception exception = context.Exception;
            var json = new JsonErrorPayload
            {
                Error = exception.Message
            };

            context.Result = new ObjectResult(json)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            //return base.OnExceptionAsync(context);
            Exception exception = context.Exception;
            var json = new JsonErrorPayload
            {
                Error = exception.Message
            };

            context.Result = new ObjectResult(json)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }
    }

    public class JsonErrorPayload
    {
        public string Error { get; set; }
    }
}
