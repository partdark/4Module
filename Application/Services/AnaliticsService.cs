using Application.Interfaces;
using Confluent.Kafka;


namespace Applications.Services
{


    public class AnaliticsService : IAnaliticsService
    {
        private readonly IProducer<string, string> _producer;

        public AnaliticsService(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public async Task SendEventAsync(string topic, string key, string message)
        {
            await _producer.ProduceAsync(topic, new Message<string, string> { Key = key, Value = message });
        }
    }
}