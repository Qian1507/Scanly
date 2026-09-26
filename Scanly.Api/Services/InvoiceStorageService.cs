using Azure.Identity;
using Azure.Storage.Blobs;
using System.Text.Json;
using Scanly.Api.Models;

namespace Scanly.Api.Services;

public class InvoiceStorageService : IInvoiceStorageService
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

        var containerName = configuration["AZURE_STORAGE_CONTAINER"];

        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new InvalidOperationException(
                "AZURE_STORAGE_CONTAINER is not configured.");
        }

        var blobServiceClient = new BlobServiceClient(
            new Uri(storageUrl),
            new DefaultAzureCredential());

        _containerClient =
            blobServiceClient.GetBlobContainerClient(containerName);
    }

    public async Task SaveAsync(
        string invoiceId,
        InvoiceResult result)
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

        var response =
            await blobClient.DownloadContentAsync();

        return response.Value.Content
            .ToObjectFromJson<InvoiceResult>();
    }

    public async Task<IReadOnlyList<InvoiceResult>> GetAllAsync()
    {
        var results = new List<InvoiceResult>();

        await foreach (var blob in _containerClient.GetBlobsAsync())
        {
            if (!blob.Name.EndsWith(".json"))
            {
                continue;
            }

            var blobClient =
                _containerClient.GetBlobClient(blob.Name);

            var response =
                await blobClient.DownloadContentAsync();

            var invoice =
                response.Value.Content
                    .ToObjectFromJson<InvoiceResult>();

            if (invoice is not null)
            {
                results.Add(invoice);
            }
        }

        return results;
    }
}