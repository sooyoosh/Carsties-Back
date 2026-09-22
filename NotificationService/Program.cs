using MassTransit;
using NotificationService.Consumers;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddMassTransit(x =>
{



    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();


    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("notif", false));


    x.UsingRabbitMq((context, cfg) =>
    {
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


builder.Services.AddSignalR();



var app = builder.Build();



app.MapHub<NotificationHub>("/notifications");



//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
