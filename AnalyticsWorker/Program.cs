using AnalyticsWorker;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<KafkaConsumerService>();

var host = builder.Build();
host.Run();