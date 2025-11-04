using ContosoPizza.Common;
using ContosoPizza.Data;
using ContosoPizza.Exceptions;
using ContosoPizza.Models.DTOs;
using ContosoPizza.Models.Entities;
using ContosoPizza.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Services;

public class PizzaService : IPizzaService
{
    private readonly PizzaDb _db;
    private readonly ILogger<PizzaService> _logger;

    public PizzaService(PizzaDb db, ILogger<PizzaService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PaginatedList<PizzaDto>> GetAllAsync(PizzaQueryDto query)
    {
        _logger.LogInformation("查询披萨列表，页码：{Page}, 页大小：{PageSize}", query.Page, query.PageSize);

        var queryable = _db.Pizzas
            .Where(p => !p.IsDeleted)
            .AsNoTracking();

        // 搜索过滤
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            queryable = queryable.Where(p => p.Name.Contains(query.SearchTerm));
        }

        // 类型过滤
        if (query.IsGlutenFree.HasValue)
        {
            queryable = queryable.Where(p => p.IsGlutenFree == query.IsGlutenFree.Value);
        }

        // 排序
        queryable = query.SortBy?.ToLower() switch
        {
            "name" => query.IsDescending 
                ? queryable.OrderByDescending(p => p.Name)
                : queryable.OrderBy(p => p.Name),
            "createdat" => query.IsDescending
                ? queryable.OrderByDescending(p => p.CreatedAt)
                : queryable.OrderBy(p => p.CreatedAt),
            _ => query.IsDescending
                ? queryable.OrderByDescending(p => p.Id)
                : queryable.OrderBy(p => p.Id)
        };

        // 获取总数
        var totalCount = await queryable.CountAsync();

        // 分页
        var items = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(p => new PizzaDto
            {
                Id = p.Id,
                Name = p.Name,
                IsGlutenFree = p.IsGlutenFree,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();

        return new PaginatedList<PizzaDto>(items, totalCount, query.Page, query.PageSize);
    }

    public async Task<PizzaDto> GetByIdAsync(int id)
    {
        _logger.LogInformation("查询披萨，ID: {Id}", id);

        var pizza = await _db.Pizzas
            .AsNoTracking()
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new PizzaDto
            {
                Id = p.Id,
                Name = p.Name,
                IsGlutenFree = p.IsGlutenFree,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (pizza == null)
        {
            _logger.LogWarning("披萨未找到，ID: {Id}", id);
            throw new NotFoundException("Pizza", id);
        }

        return pizza;
    }

    public async Task<PizzaDto> CreateAsync(CreatePizzaDto dto)
    {
        _logger.LogInformation("创建披萨：{Name}", dto.Name);

        var pizza = new Pizza
        {
            Name = dto.Name,
            IsGlutenFree = dto.IsGlutenFree,
            CreatedAt = DateTime.UtcNow
        };

        _db.Pizzas.Add(pizza);
        await _db.SaveChangesAsync();

        _logger.LogInformation("披萨创建成功，ID: {Id}", pizza.Id);

        return new PizzaDto
        {
            Id = pizza.Id,
            Name = pizza.Name,
            IsGlutenFree = pizza.IsGlutenFree,
            CreatedAt = pizza.CreatedAt,
            UpdatedAt = pizza.UpdatedAt
        };
    }

    public async Task<PizzaDto> UpdateAsync(int id, UpdatePizzaDto dto)
    {
        _logger.LogInformation("更新披萨，ID: {Id}", id);

        var pizza = await _db.Pizzas.FindAsync(id);

        if (pizza == null || pizza.IsDeleted)
        {
            _logger.LogWarning("披萨未找到，ID: {Id}", id);
            throw new NotFoundException("Pizza", id);
        }

        // 更新字段
        pizza.Name = dto.Name;
        pizza.IsGlutenFree = dto.IsGlutenFree;
        pizza.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _db.SaveChangesAsync();
            _logger.LogInformation("披萨更新成功，ID: {Id}", id);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "并发冲突，ID: {Id}", id);
            throw new BadRequestException("数据已被其他用户修改，请刷新后重试");
        }

        return new PizzaDto
        {
            Id = pizza.Id,
            Name = pizza.Name,
            IsGlutenFree = pizza.IsGlutenFree,
            CreatedAt = pizza.CreatedAt,
            UpdatedAt = pizza.UpdatedAt
        };
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("删除披萨，ID: {Id}", id);

        var pizza = await _db.Pizzas.FindAsync(id);

        if (pizza == null || pizza.IsDeleted)
        {
            _logger.LogWarning("披萨未找到，ID: {Id}", id);
            throw new NotFoundException("Pizza", id);
        }

        // 软删除
        pizza.IsDeleted = true;
        pizza.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("披萨删除成功，ID: {Id}", id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _db.Pizzas
            .AsNoTracking()
            .AnyAsync(p => p.Id == id && !p.IsDeleted);
    }
}