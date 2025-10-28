
using _4Module;
using Application.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);


// builder.Services.AddDbContext<BookContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); 
builder.Services.Configure<MySettings>(builder.Configuration.GetSection("MySettings"));



// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("BookPolicy", policity =>
{
    policity.Expire(TimeSpan.FromSeconds(60)).SetVaryByRouteValue("id");
}
    );
}
 );

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Book Api",
        Version = "v1",
        Description = "Book Api"
    });


    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();


var app = builder.Build();




app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandler = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var exeption = exceptionHandler?.Error;
        var details = new
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "Internal Server Error",
            Status = StatusCodes.Status500InternalServerError,
            deatail = "An unexpected error",
            Instance = context.Request.Path,
            TraceId = context.TraceIdentifier
        };
        Console.WriteLine($"Error {exeption?.Message}");
        Console.WriteLine($"StackTrace {exeption?.StackTrace}");

        await context.Response.WriteAsJsonAsync(details);
    });
});

app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;
    var stopwatch = Stopwatch.StartNew();
    Console.WriteLine($"Start {context.Request.Method}{context.Request.Path} time - {startTime}");

    await next();
    stopwatch.Stop();
    Console.WriteLine($"done {context.Request.Method}{context.Request.Path}, status {context.Response.StatusCode} time - {stopwatch.ToString()}");
}
);


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseOutputCache();




app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz");


app.MapControllers();

app.Run();
