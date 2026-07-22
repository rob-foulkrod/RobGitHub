using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RobGitHub.Web.Services;

namespace RobGitHub.Web.Tests;

public class AppEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public AppEndpointsTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthy()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task TodoPage_ShowsPendingNotification()
    {
        using var scope = factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITodoRepository>();
        repository.Add("Wave at the backlog");

        using var client = factory.CreateClient();
        var html = await client.GetStringAsync("/");

        Assert.Contains("pending todo", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Wave at the backlog", html, StringComparison.Ordinal);
    }
}
