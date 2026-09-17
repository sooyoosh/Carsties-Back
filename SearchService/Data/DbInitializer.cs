using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication application)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
            .FromConnectionString(application.Configuration.GetConnectionString("MongoDbConnection")));
            await DB.Index<Item>()
            .Key(x => x.Make, KeyType.Text)
            .Key(x => x.Model, KeyType.Text)
            .Key(x => x.Color, KeyType.Text)
            .CreateAsync();



            var count = await DB.CountAsync<Item>();



            Console.WriteLine($"Items already in Mongo: {count}");
            //if (count == 0)
            //{

            //    Console.WriteLine("No data - will attempt to seed");
            //    var itemData = await File.ReadAllTextAsync("Data/auctions.json");

            //    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            //    var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

            //    await DB.SaveAsync(items);

            //}



            using var scope = application.Services.CreateScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<AuctionSvcHttpClient>();

            var items = await httpClient.GetItemsForSearchDb();

            Console.WriteLine($"Items returned from AuctionService: {items.Count}");

            if (items.Count > 0) await DB.SaveAsync(items);


        }
    }
}
