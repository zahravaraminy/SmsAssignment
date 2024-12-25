using System.Collections.Concurrent;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmsRateLimiter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register SlidingWindow with a factory method that supplies the 'limit' parameter
builder.Services.AddSingleton<SmsController>(); // Registers the SmsController

builder.Services.AddSingleton<SlidingWindow>(provider => new SlidingWindow(10)); // Account-wide limit (10 messages per second)

builder.Services.AddSingleton<ConcurrentDictionary<string, SlidingWindow>>(); // Per-number limits as singleton
builder.Services.AddSingleton<ConcurrentDictionary<string, AccountMonitorModel>>();
builder.Services.AddSingleton<ConcurrentDictionary<string, List<NumberMonitorModel>>>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

  // Add CORS services to the container
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:4200") // Your frontend URL
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
    });

var app = builder.Build();

 // Enable CORS
app.UseCors("AllowFrontend");

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

