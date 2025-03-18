using Serilog.Context;
using System.Diagnostics;

namespace DoaMais.API.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await _next(context);
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            using (LogContext.PushProperty("http.response_time", elapsedMs))
            using (LogContext.PushProperty("http.request_method", context.Request.Method))
            using (LogContext.PushProperty("http.request_path", context.Request.Path))
            using (LogContext.PushProperty("http.response_status", context.Response.StatusCode))
            {
                _logger.LogInformation($"HTTP {context.Request.Method} {context.Request.Path} responded {context.Response.StatusCode} in {elapsedMs}ms");
            }
        }
    }
}
