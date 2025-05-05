using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using ToDoApi;
namespace ToDoApi.Tests.Fixtures;

using Xunit;

public class TestContextFixture : IAsyncLifetime
{
    public WebApplicationFactory<Program> Factory { get; }
    public HttpClient TestClient { get; }

    public TestContextFixture()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddDbContext<TodoDb>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

        TestClient = Factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDb>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
