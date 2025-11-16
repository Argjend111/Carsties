var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UserAuthorization();

app.MapControllers();

app.Run();