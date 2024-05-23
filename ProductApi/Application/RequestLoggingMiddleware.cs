namespace ProductApi.Application;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation($"Request [{context.Request.Method}] at {context.Request.Path} responded {context.Response.StatusCode} in {elapsedMs} ms");

            return Task.CompletedTask;
        });

        await _next(context);
    }
}
