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
    // 数据注解：https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations?utm_source=chatgpt.com
    [Key] // 指定该字段为主键
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 设置自增
    public int Id { get; set; }

    [Required] // 必需
    [MaxLength(100)] // 最大长度
    public string Name { get; set; } = string.Empty; // 给字符串一个默认值“”

    public bool IsGlutenFree { get; set; }

    /// <summary>
    /// 创建时间（数据库自动生成）
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 设置自增
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 设置当前时间为默认值

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedAt { get; set; } // 默认为 null

    /// <summary>
    /// 软删除标记
    /// </summary>
    public bool IsDeleted { get; set; } = false; //默认为 false

    /// <summary>
    /// 行版本（用于乐观并发控制）
    /// </summary>
    [Timestamp] // 把这个属性标记为 并发版本号（乐观锁）字段。
    public byte[]? RowVersion { get; set; } // 每次更新数据库都会变，用于检测并发冲突
}