using Application.DependencyInjection;
using Application.Settings;
using GrpcAuthorsService.Services;
using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<JwtSettings>(provider =>
    provider.GetRequiredService<IOptions<JwtSettings>>().Value);

builder.Services.AddDataProtection();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Infrastructure.Data.BookContext>();
    await context.Database.EnsureCreatedAsync();
}

app.MapGrpcService<AuthorServiceImpl>();

Console.WriteLine("AuthorGrpcService starting on https://localhost:7002");
app.Run("https://localhost:7002");
