using Microsoft.EntityFrameworkCore;
using Order.Application.Mappers;
using Order.Application.Handlers;
using Order.Infrastructure.Context;
using Order.Application.Extensions;
using MediatR;
using Order.API.EventBusConsumer;
using MassTransit;
using EventBus.Messages.Common;
using Serilog;
using Common.Logging;

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

// Add Controllers
builder.Services.AddControllers();

//Serilog Configuration
builder.Host.UseSerilog(Logging.ConfigureLogger);

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Order.API", Version = "v1" });
});

// Application Services (if you have custom DI extensions)
builder.Services.AddApplicationServices();

// Register DbContext
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderConnectionString")));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(OrderMappingProfile));

// Register MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CheckoutOrderCommandHandler).Assembly));

//Register Consumer class
builder.Services.AddScoped<BasketOrderConsumer>();
builder.Services.AddScoped<BasketOrderConsumerV2>();

//Register RabbitMQ / MassTransit
builder.Services.AddMassTransit(config =>
{
    // Mark this as consumer
    config.AddConsumer<BasketOrderConsumer>();
    config.AddConsumer<BasketOrderConsumerV2>();
    config.UsingRabbitMq((ct, cfg) =>
    {
        cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
        //provide the queue name with consumer settings
        cfg.ReceiveEndpoint(EventBusConstatnt.BasketCheckoutQueue, c =>
        {
            c.ConfigureConsumer<BasketOrderConsumer>(ct);
        });
        // V2 version
        cfg.ReceiveEndpoint(EventBusConstatnt.BasketCheckoutQueueV2, c =>
        {
            c.ConfigureConsumer<BasketOrderConsumerV2>(ct);
        });
    });
});
builder.Services.AddHealthChecks();

builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order.API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

