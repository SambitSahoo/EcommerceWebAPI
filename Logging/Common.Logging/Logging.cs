using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging
{
    public static class Logging
    {
        public static Action<HostBuilderContext,LoggerConfiguration> ConfigureLogger => (context, logging) =>
        {
            var env = context.HostingEnvironment;
            logging.MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", env.ApplicationName)
            .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.LifeTime", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console();

            if (context.HostingEnvironment.IsDevelopment())
            {
                logging.MinimumLevel.Override("Catalog", Serilog.Events.LogEventLevel.Debug);
                logging.MinimumLevel.Override("Basket", Serilog.Events.LogEventLevel.Debug);
                logging.MinimumLevel.Override("Discount", Serilog.Events.LogEventLevel.Debug);
                logging.MinimumLevel.Override("Order", Serilog.Events.LogEventLevel.Debug);
            }

            // Elastic Search
            var elasticUrl = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            if (!string.IsNullOrEmpty(elasticUrl))
            {
                logging.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                    IndexFormat = "ecommerce-logs-{0:yyyy.MM.dd}",
                    MinimumLogEventLevel = LogEventLevel.Debug
                });

            }
        };

    }
}