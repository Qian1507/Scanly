using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Scanly.Api.Tests.Endpoints;

public class InvoiceEndpointTests
{
    [Fact]
    public async Task PostInvoice_ReturnsOkAndId()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        using var content = new MultipartFormDataContent();

        var fileContent = new ByteArrayContent("fake invoice content"u8.ToArray());
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

        content.Add(fileContent, "file", "invoice.png");

        var response = await client.PostAsync("/invoices", content);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.Contains("\"id\"", responseBody);
    }

    [Fact]
    public async Task GetInvoiceById_ReturnsOk()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var id = Guid.NewGuid();

        var response = await client.GetAsync($"/invoices/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetInvoices_ReturnsOk()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/invoices");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}