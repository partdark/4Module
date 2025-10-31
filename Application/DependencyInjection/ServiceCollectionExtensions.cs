using Application.DTO;
using Application.Interfaces;
using Application.Services;
using Application.Validator;
using Applications.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            services.AddHttpClient("TestClient", client =>
            {
                client.BaseAddress = new Uri("https://petstore.swagger.io/");
            });
            services.AddHttpClient<IAuthorHttpService, AuthorHttpService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7134/api/Book/");
            }).AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1)))
            .AddPolicyHandler(HttpPolicyExtensions.HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(2))); 


            return services;
        }
    }
}
