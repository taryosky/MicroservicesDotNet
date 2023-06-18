using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace PlatformService.Data
{
    public class PreDb
    { 
        public static void SeedData(IApplicationBuilder app)
        {
            Console.WriteLine("--> Seeding data...");
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<AppDbContext>();
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