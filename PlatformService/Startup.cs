using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using PlatformService.Data;
using PlatformService.SyncDataServices.Http;
using PlatformService.AsyncDataServices;
using System;
using System.IO;
using PlatformService.SyncDataServices.Grpc;

namespace PlatformService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine($"--> Command service endpoint: {Configuration["CommandServicebaseUrl"]}");
            services.AddControllers();
            services.AddScoped<IPlatformRepo, PlatformRepo>();
            services.AddSingleton<IMessageBusClient, MessageBusClient>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
            });
            services.AddGrpc();

            services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            if(_env.IsProduction())
            {
                Console.WriteLine("--> Using sql server db");
                services.AddDbContext<AppDbContext>(op => op.UseSqlServer(Configuration.GetConnectionString("PlatformsConn")));
            }
            else
            {
                Console.WriteLine("---> Using inmemory db");
                services.AddDbContext<AppDbContext>(op => op.UseInMemoryDatabase("InMemory"));
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1"));
            //}

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<GrpcPlatformService>();

                endpoints.MapGet("/protos/platform.proto", async context => {
                    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
                });
            });

            PreDb.SeedData(app, env.IsProduction());
        }
    }
}
