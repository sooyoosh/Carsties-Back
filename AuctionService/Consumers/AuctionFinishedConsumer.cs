using AuctionService.Data;
using AuctionService.Entities;
using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Consumers
{
    public class AuctionFinishedConsumer: IConsumer<AuctionFinished>
    {
        private readonly AuctionDbContext _auctionDbContext;

        public AuctionFinishedConsumer(AuctionDbContext auctionDbContext)
        {
            _auctionDbContext = auctionDbContext;
        }

        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            Console.WriteLine(" -- > Consuming AuctionFinished placed");
            var auction = await _auctionDbContext.Auctions.FindAsync(Guid.Parse(context.Message.AuctionId));

            if (context.Message.ItemSold)

                auction.Winner = context.Message.Winner;
                auction.Soldamount = context.Message.Amount;

                auction.Status = auction.Soldamount > auction.ReservePrice
                ? Status.Finished : Status.ReserveNotMet;

                 await _auctionDbContext.SaveChangesAsync();
        }


    }
}
