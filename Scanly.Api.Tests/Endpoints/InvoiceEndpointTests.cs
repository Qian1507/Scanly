using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Scanly.Api.Models;
using System.Net.Http.Json;

namespace Scanly.Api.Tests.Endpoints;

public class InvoiceEndpointTests
    : IClassFixture<ScanlyWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InvoiceEndpointTests(
        ScanlyWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // --------------------------------------------------
    // POST /invoices
    // Fake analysis + fake storage
    // --------------------------------------------------

    [Fact]
    public async Task PostInvoice_ReturnsOkAndInvoiceResult()
    {
        using var content = new MultipartFormDataContent();

        var fileContent =
            new ByteArrayContent(
                "fake invoice content"u8.ToArray());

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("image/png");

        content.Add(
            fileContent,
            "file",
            "invoice.png");

        var response =
            await _client.PostAsync(
                "/invoices",
                content);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var invoice =
            await response.Content
                .ReadFromJsonAsync<InvoiceResult>();

        Assert.NotNull(invoice);
        Assert.NotEqual(Guid.Empty, invoice.Id);
        Assert.Equal(
            "invoice.png",
            invoice.FileName);
        Assert.Equal(
            "Test Vendor",
            invoice.VendorName);
    }


    // --------------------------------------------------
    // GET /invoices/{id}
    // First create invoice, then retrieve same invoice
    // --------------------------------------------------

    [Fact]
    public async Task GetInvoiceById_ReturnsOk()
    {
        var createdInvoice =
            await CreateInvoiceAsync();

        var response =
            await _client.GetAsync(
                $"/invoices/{createdInvoice.Id}");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var invoice =
            await response.Content
                .ReadFromJsonAsync<InvoiceResult>();

        Assert.NotNull(invoice);

        Assert.Equal(
            createdInvoice.Id,
            invoice.Id);

        Assert.Equal(
            createdInvoice.FileName,
            invoice.FileName);
    }


    // --------------------------------------------------
    // GET /invoices/{id}
    // Unknown invoice -> 404
    // --------------------------------------------------

    [Fact]
    public async Task GetInvoiceById_ReturnsNotFound()
    {
        var id = Guid.NewGuid();

        var response =
            await _client.GetAsync(
                $"/invoices/{id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }


    // --------------------------------------------------
    // GET /invoices
    // --------------------------------------------------

    [Fact]
    public async Task GetInvoices_ReturnsOk()
    {
        await CreateInvoiceAsync();

        var response =
            await _client.GetAsync(
                "/invoices");

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);

        var invoices =
            await response.Content
                .ReadFromJsonAsync<
                    List<InvoiceResult>>();

        Assert.NotNull(invoices);
        Assert.NotEmpty(invoices);
    }


    // --------------------------------------------------
    // Helper
    // Create invoice through the real API endpoint
    // while using fake services underneath
    // --------------------------------------------------

    private async Task<InvoiceResult> CreateInvoiceAsync()
    {
        using var content =
            new MultipartFormDataContent();

        var fileContent =
            new ByteArrayContent(
                "fake invoice content"u8.ToArray());

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue("image/png");

        content.Add(
            fileContent,
            "file",
            "invoice.png");

        var response =
            await _client.PostAsync(
                "/invoices",
                content);

        response.EnsureSuccessStatusCode();

        var invoice =
            await response.Content
                .ReadFromJsonAsync<InvoiceResult>();

        Assert.NotNull(invoice);

        return invoice;
    }
}