using CassandraDB;
using DataInterfaces;
using Messaging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<NewMessageWorker>();
builder.Services.AddSingleton<IMessageStorage, MessageStorage>();
builder.Services.AddControllers();

var app = builder.Build();
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
