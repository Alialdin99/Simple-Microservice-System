using Catalog.Service.Entities;
// using Catalog.Service.Settings;
using MassTransit;
using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Common.Settings;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddMongo(builder.Configuration)
    .AddMongoRepository<Item>("items")
    .AddMassTransitWithRabbitMQ(builder.Configuration);
                                                

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
