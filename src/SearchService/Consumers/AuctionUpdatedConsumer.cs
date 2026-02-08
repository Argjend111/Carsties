using Contracts;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;

public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
{
    public async Task Consume(ConsumeContext<AuctionUpdated> context)
    {
        Console.WriteLine("--> Consuming auction updated: " + context.Message.Id);

        var item = await DB.Find<Item>().OneAsync(context.Message.Id);

        if (item == null)
        {
            throw new MessageException(typeof(AuctionUpdated), $"Could not find auction with id: {context.Message.Id}");
        }

        item.Make = context.Message.Make;
        item.Model = context.Message.Model;
        item.Year = context.Message.Year;
        item.Color = context.Message.Color;
        item.Mileage = context.Message.Mileage;

        await item.SaveAsync();
    }
}