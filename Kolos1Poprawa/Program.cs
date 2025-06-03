
using Kolos1Poprawa.Services;

var builder = WebApplication.CreateBuilder();
builder.Services.AddControllers();
builder.Services.AddScoped<IClientService, ClientService>();
var app = builder.Build();
app.UseAuthorization();
app.MapControllers();
app.Run();