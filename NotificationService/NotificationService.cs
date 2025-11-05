using MassTransit;
using OrderContracts;

namespace NotificationService
{
    public class NotificationServiceConsumer : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var orderEvent = context.Message;
            Console.WriteLine($"NotificationService: {orderEvent.OrderId} : {orderEvent.CreatedAt}");
            await Task.CompletedTask;
           
        }
    }
}
