using Confluent.Kafka;
using Infrastructure.Data;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using OrderWorkerService.Data;
using System.Linq;
using Testcontainers.PostgreSql;
using Xunit;

namespace IntegrationTest
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");
            builder.ConfigureServices(services =>
            {
 
                var dbContextOptionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BookContext>));
                if (dbContextOptionsDescriptor != null)
                {
                    services.Remove(dbContextOptionsDescriptor);
                }
                
                var orderContextOptionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<OrderContext>));
                if (orderContextOptionsDescriptor != null)
                {
                    services.Remove(orderContextOptionsDescriptor);
                }

                services.RemoveAll(typeof(IUserStore<IdentityUser>));
                services.RemoveAll(typeof(IRoleStore<IdentityRole>));
                services.RemoveAll(typeof(UserManager<IdentityUser>));
                services.RemoveAll(typeof(RoleManager<IdentityRole>));
                services.RemoveAll(typeof(SignInManager<IdentityUser>));


                var massTransitDescriptors = services.Where(d =>
                    d.ServiceType.Namespace?.StartsWith("MassTransit", StringComparison.Ordinal) == true ||
                    d.ImplementationType?.Namespace?.StartsWith("MassTransit", StringComparison.Ordinal) == true)
                    .ToList();
                foreach (var descriptor in massTransitDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.RemoveAll(typeof(IProducer<string, string>));

                var authDescriptors = services.Where(d =>
                    d.ServiceType.Namespace?.StartsWith("Microsoft.AspNetCore.Authentication", StringComparison.Ordinal) == true)
                    .ToList();
                foreach (var descriptor in authDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.RemoveAll(typeof(IConfigureOptions<JwtBearerOptions>));
                services.RemoveAll(typeof(IPostConfigureOptions<JwtBearerOptions>));

                // Remove authentication scheme providers
                services.RemoveAll(typeof(IAuthenticationSchemeProvider));
                services.RemoveAll(typeof(IAuthenticationService));
                services.RemoveAll(typeof(IAuthenticationHandlerProvider));
                services.RemoveAll(typeof(IAuthenticationRequestHandler));

                var authServices = services.Where(s =>
               s.ServiceType.FullName.Contains("Authentication") ||
               s.ServiceType == typeof(IAuthenticationService) ||
               s.ServiceType == typeof(IAuthenticationSchemeProvider) ||
               s.ServiceType.Name.Contains("JwtBearer") ||
               s.ServiceType.Name.Contains("AuthenticationOptions"))
               .ToList();

                foreach (var service in authServices)
                {
                    services.Remove(service);
                }

                services.AddDbContext<BookContext>(options =>
                {
                    options.UseNpgsql(_postgresContainer.GetConnectionString(),
                        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__BookMigrationsHistory"));
                });
                services.AddDbContext<OrderContext>(options =>
                {
                    options.UseNpgsql(_postgresContainer.GetConnectionString(),
                        npgsqlOptions => npgsqlOptions.MigrationsHistoryTable("__OrderMigrationsHistory"));
                });
                

                services.AddIdentityCore<IdentityUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<BookContext>();


                services.AddMassTransitTestHarness(x => { });

                var mockProducer = new Mock<IProducer<string, string>>();
                services.AddSingleton(mockProducer.Object);

                services.AddAuthentication("Bearer")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Bearer", _ => { });
            });
        }

        public async Task InitializeAsync()
        {
            await _postgresContainer.StartAsync();

            using var scope = Services.CreateScope();
            var bookContext = scope.ServiceProvider.GetRequiredService<BookContext>();
            await bookContext.Database.EnsureDeletedAsync();
            await bookContext.Database.MigrateAsync();

            var orderContext = scope.ServiceProvider.GetRequiredService<OrderContext>();
            await orderContext.Database.MigrateAsync();
        }

        public new async Task DisposeAsync()
        {
            await _postgresContainer.StopAsync();
            await base.DisposeAsync();
        }
    }
}