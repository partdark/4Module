using AnalyticsWorker;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;


var builder = Host.CreateApplicationBuilder(args);


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