using System.Reflection;
using Discount.API.Services;
using Discount.Application.Handlers;
using Discount.Core.IRepo;
using Discount.Infrastructure.Extensions;
using Discount.Infrastructure.Repositories;
using Serilog;
using Common.Logging;

namespace Discount.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Add CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        //Serilog Configuration
        builder.Host.UseSerilog(Logging.ConfigureLogger);

        builder.Services.AddAuthorization();

        // Add services to the container.

        // Register AutoMapper
        builder.Services.AddAutoMapper(typeof(Program).Assembly);

        // Register MediatR
        var assemblies = new Assembly[]
        {
        Assembly.GetExecutingAssembly(),
        typeof(CreateDiscountCommandHandler).Assembly
        };
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));
        builder.Services.AddScoped<IDiscountRepo, DiscountRepository>();

        //Register gRPC
        builder.Services.AddGrpc();

        var app = builder.Build();

        //Migrate Database
        app.MigrateDatabase<Program>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAuthorization();

        app.UseRouting();

        app.MapGrpcService<DiscountService>();
        app.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client.");
        });

        app.Run();
    }
}
