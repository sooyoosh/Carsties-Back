using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumer
{
    public class BidPlacedConsumer : IConsumer<BidPlaces>
    {
        public async Task Consume(ConsumeContext<BidPlaces> context)
        {
            Console.WriteLine(" -- > Consuming bid placed");

            var auction = await DB.Find<Item>().OneAsync(context.Message.AuctionId);

          

            if(context.Message.BidStatus.Contains("Accepted")
            && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await auction.SaveAsync();
            }

               
        }
    }
}
