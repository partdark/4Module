
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Identity;
using MongoDB.Driver;
using Repository;


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



            services.AddIdentityCore<IdentityUser>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
            })
 .AddRoles<IdentityRole>()
 .AddEntityFrameworkStores<BookContext>()
 .AddDefaultTokenProviders()
 .AddRoleManager<RoleManager<IdentityRole>>()
 .AddUserManager<UserManager<IdentityUser>>();



            services.AddHostedService<AverageRatingCalculatorService>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IAuthorReportService, AuthorReportService>();
            services.AddScoped<IProductReviewRepository, ProductReviewRepository>();



            return services;
        }
    }
}
