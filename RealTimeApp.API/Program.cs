using RealTimeApp.API.SignalR.Hubs;
using Microsoft.EntityFrameworkCore;
using RealTimeApp.API.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// db
builder.Services.AddDbContext<AppDbContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("RealTimeDB"));
});
// SignalR servis olarak eklenir
builder.Services.AddSignalR();
//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        /*builder.AllowAnyOrigin();*/ // tüm client'lara izin verir.
        // bu url'e izin ver
        builder.WithOrigins("http://localhost:4021")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("CorsPolicy");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
// Middleware olarak eklenir
// hangi url'den client'lar baðlansýn
// client'lar signalr hub'ýma þu url'den baðlansýn
// client'lar bu hub'a baðlanmak istiyorsa: http://localhost/myhub ile
// client'lar hub'a baðlanýr
// api'ye çift taraflý iletiþimi saðlayacak özellik kazandýrdýk
app.MapHub<MyHub>("/myhub");

app.Run();
