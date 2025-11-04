// Models/Entities/Pizza.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoPizza.Models.Entities;

/// <summary>
/// Pizza 实体类
/// </summary>
[Table("Pizzas")]
public class Pizza
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public bool IsGlutenFree { get; set; }

    /// <summary>
    /// 创建时间（数据库自动生成）
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// 软删除标记
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// 行版本（用于乐观并发控制）
    /// </summary>
    [Timestamp]
    public byte[]? RowVersion { get; set; }
}