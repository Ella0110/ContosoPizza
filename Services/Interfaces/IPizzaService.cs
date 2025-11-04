
using ContosoPizza.Common;
using ContosoPizza.Models.DTOs;

namespace ContosoPizza.Services.Interfaces;

public interface IPizzaService
{
    Task<PaginatedList<PizzaDto>> GetAllAsync(PizzaQueryDto query);
    Task<PizzaDto> GetByIdAsync(int id);
    Task<PizzaDto> CreateAsync(CreatePizzaDto dto);
    Task<PizzaDto> UpdateAsync(int id, UpdatePizzaDto dto);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}