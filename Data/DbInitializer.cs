// Data/DbInitializer.cs
using System;
using System.Threading.Tasks;
using ContosoPizza.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContosoPizza.Data
{
    internal static class DbInitializer
    {
        internal static async Task SeedAsync(PizzaDb context)
        {
            // 可选：确保数据库已创建 / 迁移
            await context.Database.EnsureCreatedAsync();
            // 或：await context.Database.MigrateAsync();

            if (await context.Pizzas.AnyAsync())
            {
                return; // 已有数据就不再种子
            }

            var now = DateTime.UtcNow;

            context.Pizzas.AddRange(
                new Pizza
                {
                    Name = "Classic Italian",
                    IsGlutenFree = false,
                    CreatedAt = now
                },
                new Pizza
                {
                    Name = "Veggie Supreme",
                    IsGlutenFree = true,
                    CreatedAt = now
                },
                new Pizza
                {
                    Name = "Pepperoni Deluxe",
                    IsGlutenFree = false,
                    CreatedAt = now
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
