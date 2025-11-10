using AnalyticsWorker.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticsWorker
{
    public class KafkaConsumerService : BackgroundService
    {
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
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoOffsetStore = false,
                    EnableAutoCommit = false,
                };
                _consumer = new ConsumerBuilder<string, string>(config).Build();

                var mongoConnectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING") ?? "mongodb://localhost:27017";
                var client = new MongoClient(mongoConnectionString);
                var database = client.GetDatabase("analytics");
                _collection = database.GetCollection<BookEvent>("books-views");


                _consumer.Subscribe("books-views");
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                      
                        var messages = new List<ConsumeResult<string, string>>();
                        var batchSize = 10;
                        var timeout = TimeSpan.FromMilliseconds(100);

                        for (int i = 0; i < batchSize && !stoppingToken.IsCancellationRequested; i++)
                        {
                            var result = _consumer.Consume(timeout);
                            if (result != null)
                            {
                                messages.Add(result);
                            }
                            else
                            {
                                break; 
                            }
                        }

                        if (messages.Count > 0)
                        {
                           
                            await Parallel.ForEachAsync(messages,
                                new ParallelOptions
                                {
                                    MaxDegreeOfParallelism = 4,
                                    CancellationToken = stoppingToken
                                },
                                async (message, ct) =>
                                {
                                    try
                                    {
                                        var bookEvent = new BookEvent
                                        {
                                            Key = message.Message.Key,
                                            Message = message.Message.Value
                                        };

                                        await _collection.InsertOneAsync(bookEvent, cancellationToken: ct);

                                      
                                        lock (_consumer)
                                        {
                                            _consumer.StoreOffset(message);
                                        }

                                        _logger.LogInformation($"Выполнение: {message.Message.Key} - {message.Message.Value}");
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"Ошибка: {ex.Message}");
                                    }
                                });
                        }
                        else
                        {
                           
                            await Task.Delay(100, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Mongo error: {ex.Message}");
                    }
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
