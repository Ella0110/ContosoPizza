namespace ContosoPizza.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<Middleware.GlobalExceptionHandler>();
    }
}

//MiddlewareExtensions.UseGlobalExceptionHandler 是个糖：

// 让你在 Program.cs 里用一行：

// app.UseGlobalExceptionHandler();

// 来启用这个全局异常处理中间件。