using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace Aruba.Identity.FunctionalTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoContainer;

    public CustomWebApplicationFactory() => 
        _mongoContainer = new MongoDbBuilder("mongo:7.0")
            .WithCleanUp(true)
            .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, options => { });

            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMongoDatabase));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton(_ =>
            {
                var client = new MongoClient(_mongoContainer.GetConnectionString());
                return client.GetDatabase("testdb");
            });
        });
    }

    public async Task InitializeAsync() => await _mongoContainer.StartAsync();

    public async new Task DisposeAsync() => await _mongoContainer.DisposeAsync();
}