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
        /*builder.AllowAnyOrigin();*/ // t�m client'lara izin verir.
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
// hangi url'den client'lar ba�lans�n
// client'lar signalr hub'�ma �u url'den ba�lans�n
// client'lar bu hub'a ba�lanmak istiyorsa: http://localhost/myhub ile
// client'lar hub'a ba�lan�r
// api'ye �ift tarafl� ileti�imi sa�layacak �zellik kazand�rd�k
app.MapHub<MyHub>("/myhub");

app.Run();
