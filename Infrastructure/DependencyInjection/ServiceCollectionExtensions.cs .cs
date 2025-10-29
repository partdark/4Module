
using Infastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;

using MongoDB.Driver;
using Domain.Interfaces;
using Infrastructure.Services;
using Infrastructure.Interfaces;


namespace Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            services.AddStackExchangeRedisCache(options => { options.Configuration = "localhost:6379"; });

            services.AddSingleton<IMongoClient>(new MongoClient("mongodb://localhost:27017"));

            services.AddDbContext<BookContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddHostedService<AverageRatingCalculatorService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IAuthorReportService, AuthorReportService>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            


            return services;
        }
    }
}
