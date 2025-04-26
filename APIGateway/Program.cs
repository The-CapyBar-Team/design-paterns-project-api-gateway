using AllProductsService.Protos;
using APIGateway.Services;
using APIGateway.Utils;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

Env.Load();

builder.Services.AddSingleton<ResponseTracker>(_ =>
{
    return new ResponseTracker(TimeSpan.FromSeconds(10));
});

builder.Services.AddSingleton<RabbitMQService>(serviceProvider =>
{
    var rabbitMQHost = Env.GetString("RABBITMQ_HOST");
    var rabbitMQPort = Env.GetString("RABBITMQ_PORT");
    var rabbitMQUser = Env.GetString("RABBITMQ_USER");
    var rabbitMQPassword = Env.GetString("RABBITMQ_PASSWORD");

    var rabbitMQService = new RabbitMQService(rabbitMQHost, rabbitMQPort, rabbitMQUser, rabbitMQPassword);

    rabbitMQService.DeclareQueue("AllproductsRequests").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("AllproductsResponses").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("AddToCartRequests").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("RemoveFromCartRequests").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("ProductStatusRequests").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("BuyProductRequests").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("AddProductDBRequests").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("UpdateProductDBRequests").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("QueuePositionUpdates").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("LostProducts").GetAwaiter().GetResult();
    rabbitMQService.DeclareQueue("ProductStatusUpdates").GetAwaiter().GetResult();

    rabbitMQService.SubscribeToQueue("AllproductsResponses", resp =>
    {
        var responseTracker = serviceProvider.GetService<ResponseTracker>();
        var products = AllProductsResponse.Parser.ParseFrom(resp);
        responseTracker?.CompleteRequest(products.RequestId, resp);
    });
    return rabbitMQService;
});

builder.Services.AddHostedService<NotificationService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
    {
        policy.WithOrigins("http://localhost:5185")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHub<NotificationHub>("api/notifications");

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowBlazorApp");

app.MapControllers();

app.Run();
