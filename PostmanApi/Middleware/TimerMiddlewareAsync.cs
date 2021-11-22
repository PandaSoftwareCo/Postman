using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PostmanApi.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class TimerMiddlewareAsync
    {
        private const string X_RESPONSE_TIME_MS = "X_Response_Time_ms";
        private readonly RequestDelegate _next;
        private ILogger<TimerMiddlewareAsync> _logger;

        public TimerMiddlewareAsync(RequestDelegate next, ILogger<TimerMiddlewareAsync> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task InvokeAsync(HttpContext httpContext)
        {
            var watch = new Stopwatch();
            watch.Start();
            httpContext.Response.OnStarting(() =>
            {
                watch.Stop();
                var responseTime = watch.ElapsedMilliseconds;
                httpContext.Response.Headers[X_RESPONSE_TIME_MS] = responseTime.ToString();
                _logger.LogInformation($"ELAPSED: {responseTime}");
                return Task.CompletedTask;
            });
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TimerMiddlewareAsyncExtensions
    {
        public static IApplicationBuilder UseTimerMiddlewareAsync(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TimerMiddlewareAsync>();
        }
    }
}
