using Amazon.Runtime.Internal.Util;
using Domain.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace Infrastructure.Services
{
    public class AverageRatingCalculatorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDistributedCache _cache;
        
        public AverageRatingCalculatorService(
            IServiceScopeFactory scopeFactory,
            IDistributedCache cache)
        {
            _scopeFactory = scopeFactory;
            _cache = cache;
           
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested) { 
            try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var reviewRepossitory = scope.ServiceProvider.GetRequiredService<IProductReviewRepository>();
                    var allReviews = await reviewRepossitory.GetAllAsync();
                    var averageRating = allReviews.GroupBy(r => r.BookId).Select(g => new
                    {
                        ProductId = g.Key,
                        AverageRating = g.Average(r => r.Rating)
                    }
                    );
                    foreach (var raiting in averageRating)
                    {
                        var cachKey = $"rating:{raiting.ProductId}";
                        await _cache.SetStringAsync(cachKey, raiting.AverageRating.ToString());
                    }
                    Console.WriteLine("Calculating rating");
                    Console.WriteLine($"Found {averageRating.Count()} products with ratings");
                }
                catch
                {
                    Console.WriteLine("Calculating rating error");

                }
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
