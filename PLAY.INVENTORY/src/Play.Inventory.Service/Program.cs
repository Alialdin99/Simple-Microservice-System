using MassTransit;
using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;


var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
builder.Services.AddMongo(configuration)
                .AddMongoRepository<InventoryItem>("invenetoryitems")
                .AddMongoRepository<CatalogItem>("catalogitems")
                .AddMassTransitWithRabbitMQ(builder.Configuration);

builder.Services.AddHttpClient<CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5062");
})
.AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
    5,
    retryAttempt => TimeSpan.FromSeconds(retryAttempt)
))
.AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
    3,
    TimeSpan.FromSeconds(15)
))
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
// Add Controllers
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
