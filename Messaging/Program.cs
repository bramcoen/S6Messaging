using Messaging;
using Messaging.Services;
using MongoDBRepository;
using Prometheus;
using StorageInterfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => {
    options.AddPolicy("default", build => build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader());
    //Check should be handled by the gateway
});
builder.Services.AddHostedService<NewMessageWorker>();
builder.Services.AddHostedService<UserWorker>();
builder.Services.AddSingleton<IUserStorage, UserStorage>();
builder.Services.AddSingleton<IMessageStorage, MessageStorage>();
builder.Services.AddSingleton<RabbitMQService>();
builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.UseCors("default");
app.UseEndpoints(endpoints =>
{
    endpoints.MapMetrics();
});
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();