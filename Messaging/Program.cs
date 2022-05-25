using Messaging;
using Messaging.Services;
using MongoDBRepository;
using StorageInterfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<NewMessageWorker>();
builder.Services.AddHostedService<UserWorker>();
builder.Services.AddSingleton<IUserStorage, UserStorage>();
builder.Services.AddSingleton<IMessageStorage, MessageStorage>();
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddControllers();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();