using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Scanly.Api.Tests.Endpoints;

public class HealthEndpointTests
{
    [Fact(Skip = "Requires Azure Document Intelligence integration")]
    public async Task GetHealth_ReturnsOk()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}