using Application.DependencyInjection;
using Application.Settings;
using GrpcAuthorsService.Services;
using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using System;

var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.WithProperty("Service", "grpc-book-service")
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.GrafanaLoki(
        builder.Configuration["Loki:Url"] ?? "http://loki:3100",
        labels: new[] { new LokiLabel { Key = "service", Value = "grpc-author-service" } })
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddGrpc();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<JwtSettings>(provider =>
    provider.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.AddDataProtection();
var meter = new System.Diagnostics.Metrics.Meter("grpc-author-service");
var bookCounter = meter.CreateCounter<int>("books.created", "books", "Number of books created");
builder.Services.AddSingleton(bookCounter);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Data.BookContext>();
    await context.Database.EnsureCreatedAsync();
}

app.MapGrpcService<AuthorServiceImpl>();

//Console.WriteLine("AuthorGrpcService starting on https://localhost:7002");
//app.Run("https://localhost:7002");
app.Run();  
