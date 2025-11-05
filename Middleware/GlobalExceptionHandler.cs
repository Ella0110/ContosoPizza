using System.Net;
using System.Text.Json;
using ContosoPizza.Common;
using ContosoPizza.Exceptions;

namespace ContosoPizza.Middleware;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    // 把整个请求包在 try/catch 里，统一处理所有未捕获异常。
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            //  把请求交给“下一个中间件/后续管道”（包括 Controller）。如果整个后续处理过程都正常，就啥也不做。
            await _next(context); 
        }
        catch (Exception ex) // 如果后面任何地方抛出了 没被处理的异常，都会被这里这个 catch 抓住。
        {
            _logger.LogError(ex, "发生未处理的异常：{Message}", ex.Message); // 打 log
            await HandleExceptionAsync(context, ex); // 抛异常
        }
    }

    // 根据异常，写 HTTP 响应
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json"; // 返回的是 JSON。

        var response = exception switch
        {
            NotFoundException notFoundEx => new
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Response = ApiResponse<object>.FailureResponse(
                    notFoundEx.Message,
                    new List<string> { notFoundEx.Message }
                )
            },
            BadRequestException badRequestEx => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Response = ApiResponse<object>.FailureResponse(
                    badRequestEx.Message,
                    new List<string> { badRequestEx.Message }
                )
            },
            ValidationException validationEx => new
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Response = ApiResponse<object>.FailureResponse(
                    "验证失败",
                    validationEx.Errors
                )
            },
            _ => new
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Response = ApiResponse<object>.FailureResponse(
                    "服务器内部错误",
                    new List<string> { exception.Message }
                )
            }
        };

        // 设置这次 HTTP 响应的状态码
        context.Response.StatusCode = response.StatusCode;

        var options = new JsonSerializerOptions
        {
            // 把属性名从 C# 的 PascalCase（StatusCode）转成 JSON 常用的 camelCase（statusCode）。
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(
            // 这里只序列化 response.Response 为 json，而不是整个 response。response 还有 StatusCode 字段已经用来设置 
            // context.Response.StatusCode 了，没必要再写到 body 里。
            JsonSerializer.Serialize(response.Response, options)
        );
    }
}