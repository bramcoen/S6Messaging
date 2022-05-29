using Messaging;
using Messaging.Services;
using MongoDBRepository;
using Prometheus;
using StorageInterfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<NewMessageWorker>();
builder.Services.AddHostedService<UserWorker>();
builder.Services.AddSingleton<IUserStorage, UserStorage>();
builder.Services.AddSingleton<IMessageStorage, MessageStorage>();
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
});
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();