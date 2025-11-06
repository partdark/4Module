using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsWorker
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly ILogger<KafkaConsumerService> _logger;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger)
        {

            _logger = logger;
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "analitics-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };
            _consumer = new ConsumerBuilder<string, string>(config).Build();

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("books-views");
            while (stoppingToken.IsCancellationRequested) {
                try
                {
                    var result = _consumer.Consume(stoppingToken);
                    _logger.LogInformation($"New view {result.Message.Key} - {result.Message.Value}");
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Error {ex.Error.Reason}");
                }
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
