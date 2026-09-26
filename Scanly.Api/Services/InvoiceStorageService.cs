using Azure.Identity;
using Azure.Storage.Blobs;
using System.Text.Json;
using Scanly.Api.Models;

namespace Scanly.Api.Services;

public class InvoiceStorageService
{
    private readonly BlobContainerClient _containerClient;

    public InvoiceStorageService(IConfiguration configuration)
    {
        var storageUrl = configuration["AZURE_STORAGE_URL"];

        if (string.IsNullOrWhiteSpace(storageUrl))
        {
            throw new InvalidOperationException(
                "AZURE_STORAGE_URL is not configured.");
        }

        var blobServiceClient = new BlobServiceClient(
            new Uri(storageUrl),
            new DefaultAzureCredential());

        _containerClient =
            blobServiceClient.GetBlobContainerClient("invoices");
    }

    public async Task SaveAsync(string invoiceId, InvoiceResult result)
    {
        var blobClient =
            _containerClient.GetBlobClient($"{invoiceId}.json");

        var json = JsonSerializer.Serialize(
            result,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        await blobClient.UploadAsync(
            BinaryData.FromString(json),
            overwrite: true);
    }

    public async Task<InvoiceResult?> GetAsync(string invoiceId)
    {
        var blobClient =
            _containerClient.GetBlobClient($"{invoiceId}.json");

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var response = await blobClient.DownloadContentAsync();

        return response.Value.Content
            .ToObjectFromJson<InvoiceResult>();
    }
}