using MassTransit;
using OrderContracts;

namespace InventoryService
{
    public class InventoryServiceConsumer : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var orderEvent = context.Message;
            Console.WriteLine($"InventoryService: {orderEvent.OrderId} : {orderEvent.CreatedAt}");
            await Task.CompletedTask;

        }
    }
}