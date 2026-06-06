using System.Reflection;
using Catalog.Application.Handlers;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Text.Json.Serialization;
using Serilog;
using Common.Logging;

namespace Catalog.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // ✅ Add CORS policy

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

            // ✅ Controllers with JSON options
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            // ✅ API Versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            // ✅ Versioned API Explorer for Swagger
            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddEndpointsApiExplorer();

            // ✅ XML Comments for Swagger
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(xmlPath);
            });

            // ✅ AutoMapper
            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            // ✅ MediatR
            var assemblies = new[]
            {
                Assembly.GetExecutingAssembly(),
                typeof(GetAllBrandsHandler).Assembly
            };
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

            // ✅ Application Services
            builder.Services.AddScoped<ICatalogContext, CatalogContext>();
            builder.Services.AddScoped<CatalogContext>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IBrandRepository, ProductRepository>();
            builder.Services.AddScoped<ITypesRepository, ProductRepository>();

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // ✅ Swagger UI with versioning
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                                                $"Catalog.API {description.ApiVersion}");
                    }
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
