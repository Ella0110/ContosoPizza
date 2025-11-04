// Controllers/PizzaController.cs
using ContosoPizza.Common;
using ContosoPizza.Models.DTOs;
using ContosoPizza.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPizza.Controllers;

/// <summary>
/// 披萨管理 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PizzaController : ControllerBase
{
    private readonly IPizzaService _pizzaService;
    private readonly ILogger<PizzaController> _logger;

    public PizzaController(IPizzaService pizzaService, ILogger<PizzaController> logger)
    {
        _pizzaService = pizzaService;
        _logger = logger;
    }

    /// <summary>
    /// 获取披萨列表（支持分页、搜索、过滤）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>披萨分页列表</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<PizzaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PaginatedList<PizzaDto>>>> GetAll([FromQuery] PizzaQueryDto query)
    {
        _logger.LogInformation("请求披萨列表");
        var result = await _pizzaService.GetAllAsync(query);
        return Ok(ApiResponse<PaginatedList<PizzaDto>>.SuccessResponse(result));
    }

    /// <summary>
    /// 根据 ID 获取披萨
    /// </summary>
    /// <param name="id">披萨 ID</param>
    /// <returns>披萨详情</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PizzaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<PizzaDto>>> Get(int id)
    {
        _logger.LogInformation("请求披萨详情，ID: {Id}", id);
        var result = await _pizzaService.GetByIdAsync(id);
        return Ok(ApiResponse<PizzaDto>.SuccessResponse(result));
    }

    /// <summary>
    /// 创建新披萨
    /// </summary>
    /// <param name="dto">创建披萨 DTO</param>
    /// <returns>创建的披萨</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PizzaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PizzaDto>>> Create([FromBody] CreatePizzaDto dto)
    {
        _logger.LogInformation("请求创建披萨");
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "验证失败", 
                ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            ));
        }

        var result = await _pizzaService.CreateAsync(dto);
        
        return CreatedAtAction(
            nameof(Get), 
            new { id = result.Id }, 
            ApiResponse<PizzaDto>.SuccessResponse(result, "创建成功")
        );
    }

    /// <summary>
    /// 更新披萨
    /// </summary>
    /// <param name="id">披萨 ID</param>
    /// <param name="dto">更新披萨 DTO</param>
    /// <returns>更新后的披萨</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PizzaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PizzaDto>>> Update(int id, [FromBody] UpdatePizzaDto dto)
    {
        _logger.LogInformation("请求更新披萨，ID: {Id}", id);
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.FailureResponse(
                "验证失败", 
                ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList()
            ));
        }

        var result = await _pizzaService.UpdateAsync(id, dto);
        return Ok(ApiResponse<PizzaDto>.SuccessResponse(result, "更新成功"));
    }

    /// <summary>
    /// 删除披萨（软删除）
    /// </summary>
    /// <param name="id">披萨 ID</param>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        _logger.LogInformation("请求删除披萨，ID: {Id}", id);
        await _pizzaService.DeleteAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(null, "删除成功"));
    }

    /// <summary>
    /// 检查披萨是否存在
    /// </summary>
    /// <param name="id">披萨 ID</param>
    [HttpHead("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Exists(int id)
    {
        var exists = await _pizzaService.ExistsAsync(id);
        return exists ? Ok() : NotFound();
    }
}