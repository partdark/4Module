using AnalyticsWorker.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Text;

namespace AnalyticsWorker
{
    public class KafkaConsumerService : BackgroundService
    {
        private static readonly ActivitySource ActivitySource = new("analytics-worker");
        private IConsumer<string, string> _consumer;
        private readonly ILogger<KafkaConsumerService> _logger;
        private IMongoCollection<BookEvent> _collection;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
            try
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "localhost:9092",
                    GroupId = "analitics-group",
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                _consumer = new ConsumerBuilder<string, string>(config).Build();

                var mongoConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? "mongodb://localhost:27017";
                var client = new MongoClient(mongoConnectionString);
                var database = client.GetDatabase("analytics");
                _collection = database.GetCollection<BookEvent>("books-views");

                _consumer.Subscribe("books-views");

                while (!stoppingToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(stoppingToken);

                    var propagator = Propagators.DefaultTextMapPropagator;
                    var parentContext = propagator.Extract(default, consumeResult.Message.Headers, (headers, key) =>
                    {
                        var header = headers?.FirstOrDefault(h => h.Key == key);
                        return header != null ? new[] { Encoding.UTF8.GetString(header.GetValueBytes()) } : Array.Empty<string>();
                    });

                    using var activity = ActivitySource.StartActivity("kafka.consume", ActivityKind.Consumer, parentContext.ActivityContext);
                    activity?.SetTag("messaging.system", "kafka");
                    activity?.SetTag("messaging.destination", "books-views");

                    var bookEvent = new BookEvent
                    {
                        Key = consumeResult.Message.Key ?? "",
                        Message = consumeResult.Message.Value,
                        TimeStamp = DateTime.UtcNow
                    };

                    using var mongoActivity = ActivitySource.StartActivity("mongodb.insert");
                    mongoActivity?.SetTag("db.system", "mongodb");
                    await _collection.InsertOneAsync(bookEvent);

                    _logger.LogInformation($"Processed: {bookEvent.Key} - {bookEvent.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
        }

        public override void Dispose()
        {
            _consumer?.Close();
            _consumer?.Dispose();
            base.Dispose();
        }
    }
}
