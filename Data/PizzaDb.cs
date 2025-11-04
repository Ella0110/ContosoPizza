using ContosoPizza.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Data;
public class PizzaDb : DbContext
{
    public PizzaDb(DbContextOptions options) : base(options) {}
    public DbSet<Pizza> Pizzas {get; set;} = null!;
}