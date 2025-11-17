using AnalyticsWorker;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.Grafana.Loki;


var builder = Host.CreateApplicationBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.WithProperty("Service", "analytics-worker")
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(
        Environment.GetEnvironmentVariable("Loki__Url") ?? "http://loki:3100",
        labels: new[] { new LokiLabel { Key = "service", Value = "analytics-worker" } })
    .CreateLogger();

builder.Services.AddSerilog();


builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("analytics-worker"))
    .WithTracing(b => b
        .AddSource("Confluent.Kafka")
        .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources")
        .AddSource("analytics-worker")
        .AddMongoDBInstrumentation()
        .AddZipkinExporter(options =>
    options.Endpoint = new Uri(Environment.GetEnvironmentVariable("ZIPKIN_ENDPOINT") ?? "http://zipkin:9411/api/v2/spans")
    )
);


builder.Services.AddHostedService<KafkaConsumerService>();

var host = builder.Build();
host.Run();