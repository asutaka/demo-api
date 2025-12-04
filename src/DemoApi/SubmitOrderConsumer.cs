using MassTransit;

namespace DemoApi
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            var msg = context.Message;
            Console.WriteLine($"[x] Received OrderId={msg.OrderId}, Product={msg.Product}, Quantity={msg.Quantity}");

            await Task.CompletedTask;
        }
    }
}
