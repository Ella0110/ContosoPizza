namespace ContosoPizza.Models.DTOs;

/// <summary>
/// Pizza 查询响应 DTO
/// </summary>
public record PizzaDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsGlutenFree { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}