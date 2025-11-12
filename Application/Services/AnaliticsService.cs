using Application.Interfaces;
using Confluent.Kafka;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Text;

namespace Applications.Services
{
    public class AnaliticsService : IAnaliticsService
    {
        private readonly IProducer<string, string> _producer;
        private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
        private static readonly ActivitySource ActivitySource = new("kafka-producer");

        public AnaliticsService(IProducer<string, string> producer)
        {
            _producer = producer;
        }

        public async Task SendEventAsync(string topic, string key, string message)
        {
            using var activity = ActivitySource.StartActivity("kafka.produce", ActivityKind.Producer);
            activity?.SetTag("messaging.system", "kafka");
            activity?.SetTag("messaging.destination", topic);

            var headers = new Headers();

            if (Activity.Current != null)
            {
                Propagator.Inject(new PropagationContext(Activity.Current.Context, Baggage.Current), headers, (hdrs, k, v) =>
                {
                    hdrs.Add(k, Encoding.UTF8.GetBytes(v));
                });
            }

            await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = message,
                Headers = headers
            });
        }
    }
}
