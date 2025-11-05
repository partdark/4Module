using MassTransit;
using OrderContracts;
using OrderWorkerService.Data;
using OrderWorkerService.Models;

namespace OrderWorkerService
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrderCommand>
    {
        private readonly OrderContext _dbContext;
        private readonly IBus _bus;

        public SubmitOrderConsumer(OrderContext dbContext, IBus bus)
        {
            _dbContext = dbContext;
            _bus = bus;

        }

        public async Task Consume(ConsumeContext<SubmitOrderCommand> context)
        {
            var command = context.Message;

            var order = new Order
            {
                Id = command.OrderId,
                Items = command.Items.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = command.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };

            _dbContext.Orders.Add(order);
           
            await _dbContext.SaveChangesAsync();

            await _bus.Publish(new OrderCreatedEvent(order.Id, order.CreatedAt, command.Items));


        }
    }
}
