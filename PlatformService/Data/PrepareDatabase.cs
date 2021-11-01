using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PrepareDatabase
    {
        public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
            }
        }

        private static void SeedData(AppDbContext context, bool isProduction)
        {
            if (isProduction)
            {
                try
                {
                    Console.WriteLine("=== Attempting EF migrations");
                    context.Database.Migrate();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"=== EF migrations failed {ex.Message}");
                }
            }

            if (!context.Platforms.Any())
            {
                Console.WriteLine("=== Seeding");
                context.Platforms.AddRange(
                    new Platform { Name = ".NET", Publisher = "Free", Cost = 0 },
                    new Platform { Name = "SQL Server", Publisher = "Free", Cost = 0 },
                    new Platform { Name = "Kubernetes", Publisher = "Free", Cost = 0 },
                    new Platform { Name = "Linux", Publisher = "Free", Cost = 0 }
                );

                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("=== Data exists, so not seeding");
            }
        }
    }
}