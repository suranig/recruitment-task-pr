using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Articles.API;

namespace Articles.IntegrationTests.Controllers;

public class HealthCheckControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthCheckControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}

public class HealthCheckResponse
{
    public string Status { get; set; } = default!;
} 