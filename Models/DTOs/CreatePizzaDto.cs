using System.ComponentModel.DataAnnotations;

namespace ContosoPizza.Models.DTOs;

/// <summary>
/// 创建 Pizza 请求 DTO
/// </summary>
public record CreatePizzaDto
{
    [Required(ErrorMessage = "披萨名称不能为空")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "名称长度必须在 2-100 个字符之间")]
    public string Name { get; init; } = string.Empty;

    public bool IsGlutenFree { get; init; }
}