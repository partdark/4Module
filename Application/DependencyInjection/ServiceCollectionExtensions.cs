using Application.DTO;
using Application.Interfaces;
using Application.Services;
using Application.Validator;
using Applications.Services;
using Confluent.Kafka;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;

using Polly.Fallback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka.Extensions.OpenTelemetry;

using Polly.Retry;
using Polly.CircuitBreaker;

using Polly.Simmy;

namespace Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IValidator<CreateBookDTO>, CreateBookDTOValidator>();
            services.AddScoped<JwtService>();
            services.AddScoped<IAnaliticsService, AnaliticsService>();
            services.AddSingleton<IProducer<string, string>>(provider =>
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "localhost:9092"
                };
                return new ProducerBuilder<string, string>(config)
                    .SetKeySerializer(Serializers.Utf8)
                    .SetValueSerializer(Serializers.Utf8)
                    .Build();
            });
            services.AddHttpClient("TestClient", client =>
            {
                client.BaseAddress = new Uri("https://petstore.swagger.io/");
            });

            services.AddHttpClient("TestClient", client =>
            {
                client.BaseAddress = new Uri("https://petstore.swagger.io/");
            });

            var httpClientBuilder = services.AddHttpClient<IAuthorHttpService, AuthorHttpService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7134/api/Book/");
            });

            httpClientBuilder.AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = 5;
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(2);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            services.AddHttpClient("TestClient", client =>
            {
                client.BaseAddress = new Uri("https://petstore.swagger.io/");
            });

            services.AddHttpClient<IAuthorHttpService, AuthorHttpService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7134/api/Book/");
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = 5;
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(2);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
            });

            return services;
        }
    }
}

