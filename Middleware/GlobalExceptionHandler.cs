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

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发生未处理的异常：{Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

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

        context.Response.StatusCode = response.StatusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response.Response, options)
        );
    }
}