using MassTransit;
using OrderContracts;

namespace OrderWorkerService_
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrderCommand>
    {
        async Task IConsumer<SubmitOrderCommand>.Consume(ConsumeContext<SubmitOrderCommand> context)
        {
            var command = context.Message;
             

            await Task.CompletedTask;
        }
    }
}
