using ContosoPizza.Data;
using ContosoPizza.Models.Entities;
using ContosoPizza.Extensions;
using ContosoPizza.Services;
using ContosoPizza.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 配置日志
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// 添加服务到容器
builder.Services.AddControllers();

// 配置数据库
var connectionString = builder.Configuration.GetConnectionString("PizzaDataBase");
builder.Services.AddDbContext<PizzaDb>(options =>
{
    options.UseSqlServer(connectionString);
    
    // 开发环境配置
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// 注册服务（依赖注入）
builder.Services.AddScoped<IPizzaService, PizzaService>();

// 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 配置 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ContosoPizza API",
        Version = "v1",
        Description = "企业级披萨管理 API",
        Contact = new OpenApiContact
        {
            Name = "Support Team",
            Email = "support@contosopizza.com"
        }
    });

    // 添加 XML 注释支持
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// // 配置健康检查
// builder.Services.AddHealthChecks()
//     .AddDbContextCheck<PizzaDb>();

var app = builder.Build();

// 配置 HTTP 请求管道

// 全局异常处理（必须在最前面）
app.UseGlobalExceptionHandler();

// 开发环境配置
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ContosoPizza API V1");
        c.RoutePrefix = string.Empty; // 设置 Swagger UI 为根路径
    });
}

// HTTPS 重定向
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// 静态文件
app.UseStaticFiles();

// 路由
app.UseRouting();

// 认证和授权
app.UseAuthentication();
app.UseAuthorization();

// 健康检查端点
// app.MapHealthChecks("/health");

// 映射控制器
app.MapControllers();

// 数据库初始化（可选）
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<PizzaDb>();
        
        // 应用待处理的迁移
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
        
        // 添加种子数据（仅在开发环境）
        if (app.Environment.IsDevelopment())
        {
            await DbInitializer.SeedAsync(context);
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "数据库初始化时发生错误");
    }
}

app.Run();