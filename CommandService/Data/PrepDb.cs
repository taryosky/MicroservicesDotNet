using Microsoft.AspNetCore.Builder;
using System.Collections.Generic;
using CommandService.Models;
using System;
using Microsoft.Extensions.DependencyInjection;
using CommandService.SyncDataServices.Grpc;
namespace CommandService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();
            var grpcClient = scope.ServiceProvider.GetService<IPlatformDataClient>();
            var platforms = grpcClient.ReturnAllPlatforms();

            SeedData(scope.ServiceProvider.GetService<ICommandRepo>(), platforms);
        }

        public static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
        {
            Console.WriteLine("---> Seeding new platforms");
            foreach(var plat in platforms)
            {
                if(!repo.PlatformExternaIdExist(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                }

                repo.SaveChanges();
            }
        }
    }
}