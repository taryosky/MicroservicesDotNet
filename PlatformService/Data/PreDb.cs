using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace PlatformService.Data
{
    public class PreDb
    { 
        public static void SeedData(IApplicationBuilder app, bool isProd)
        {
            
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if(isProd)
            {
                Console.WriteLine("---> Attempting to apply migrations");
                try
                {
                    context.Database.Migrate();
                    Console.WriteLine("---> Migrations applied successfully");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }

            if(!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding data...");
                context.Platforms.AddRange(
                new Models.Platform{Name = "EF Core", Publisher = "Microsoft", Cost = "Free"},
                new Models.Platform{Name = "Dotnet", Publisher = "Microsoft", Cost = "Free"},
                new Models.Platform{Name = "SQL Server", Publisher = "Microsoft", Cost = "Free"},
                new Models.Platform{Name = "Kubernetes", Publisher = "Cloud native computing foundation", Cost = "Free"}
                );
                context.SaveChanges();
                Console.WriteLine("--> Data seeding complete.");
            }
            
        }
    }
}