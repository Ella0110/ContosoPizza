namespace ContosoPizza.Models.DTOs;

/// <summary>
/// Pizza 查询参数 DTO（用于分页和过滤）
/// </summary>
public record PizzaQueryDto
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public bool? IsGlutenFree { get; init; }
    public string SortBy { get; init; } = "Id";
    public bool IsDescending { get; init; } = false;
}