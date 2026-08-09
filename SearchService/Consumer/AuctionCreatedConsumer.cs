using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumer
{
    public class AuctionCreatedConsumer: IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine($"Received AuctionCreated: {context.Message.Make}");

            var message = context.Message;

            var item = new Item
            {
                ID = message.Id.ToString(),
                Make = message.Make,
                Model = message.Model,
                Color = message.Color,
                Year = message.Year,
                UpdatedAt = DateTime.UtcNow
            };

            await DB.SaveAsync(item);
        }
    }
}
