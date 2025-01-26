
namespace project_core.Middlewares;

public class LoggerMiddlewares
{
    private readonly RequestDelegate _next;
    private readonly ILogger logger;

    public LoggerMiddlewares(RequestDelegate next, ILogger<LoggerMiddlewares> logger)
    {
        _next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation($"Handling request: {context.Request.Method} {context.Request.Path}");

        await _next(context);

        logger.LogInformation($"Finished handling request: {context.Request.Method} {context.Request.Path}, Response Status: {context.Response.StatusCode}");
    }
}

public static class LoggerMiddlewatesExtensions
{
    public static IApplicationBuilder UseLoggerMiddlewates(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggerMiddlewares>();
    }
}
