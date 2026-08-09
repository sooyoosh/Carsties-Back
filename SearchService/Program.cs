using MongoDB.Driver;
using MongoDB.Entities;
using Polly.Extensions.Http;
using Polly;
using SearchService.Data;
using SearchService.Models;
using SearchService.Services;
using System.Net;
using MassTransit;
using SearchService.Consumer;
using SearchService.RequestHelper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5, 5));

            e.ConfigureConsumer<AuctionCreatedConsumer>(context);

        });





        cfg.Host(
           builder.Configuration["RabbitMQ:Host"],
           "/",
           h =>
           {
               h.Username(builder.Configuration["RabbitMQ:Username"]);
               h.Password(builder.Configuration["RabbitMQ:Password"]);
           });
        cfg.ConfigureEndpoints(context);

    });

});
    builder.Services.AddAutoMapper(typeof(MappingProfiles));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//await DB.InitAsync("SearchDb", MongoClientSettings
//.FromConnectionString(builder.Configuration.GetConnectionString("MongoDbConnection")));
//await DB.Index<Item>()
//.Key(x => x.Make, KeyType.Text)
//.Key(x => x.Model, KeyType.Text)
//.Key(x => x.Color, KeyType.Text)
//.CreateAsync();
app.Lifetime.ApplicationStarted.Register(async () =>
{

        try
        {
            await DbInitializer.InitDb(app);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

});



app.Run();
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
=> HttpPolicyExtensions
.HandleTransientHttpError()
.OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
.WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));