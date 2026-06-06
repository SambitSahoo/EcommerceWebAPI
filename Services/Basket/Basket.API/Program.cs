using System.Reflection;
using Basket.Application.Handlers;
using Basket.Core.Repo;
using Basket.Infra.Repositories;
using Basket.Application.GrpcService;
using Discount.Grpc.Protos;
using MassTransit;
using Serilog;
using Common.Logging;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Swashbuckle.AspNetCore.SwaggerGen;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

builder.Services.AddControllers();

builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConnection = Environment.GetEnvironmentVariable("CacheSettings__ConnectionString")
                          ?? builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");

    options.Configuration = redisConnection;
});

//Add API Versioning
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion =new  ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Basket.API", Version = "v1" });
    c.SwaggerDoc("v2", new() { Title = "Basket.API", Version = "v2" });

    //configure swagger to use versioning
    c.DocInclusionPredicate((vesrion, ApiDescription) =>
    {
        if (!ApiDescription.TryGetMethodInfo(out MethodInfo methodInfo))
        {
            return false;
        }
        var vesrions = methodInfo.DeclaringType?.GetCustomAttributes(true)
        .OfType<ApiVersionAttribute>().SelectMany(attr => attr.Versions);
        return vesrions?.Any(v => $"v{v.ToString()}" == vesrion) ?? false;
    });

});

//Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
//Register MediatR
var assemblies = new Assembly[]
{
    Assembly.GetExecutingAssembly(),
    typeof(CreateShoppingCartCommandHandler).Assembly
};
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(assemblies);
});
//Register Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});
//Register Application Services
builder.Services.AddScoped<IBasketRepo, BasketRepository>();
//Register Grpc Services
builder.Services.AddScoped<DiscountGrpcService>();
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(cfg =>
{
    cfg.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]);
});

//Register RabbitMQ
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ct, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    });
});
builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Basket.API v2"); 
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();

