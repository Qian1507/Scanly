using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace Scanly.Api.Services;

public class DocumentIntelligenceService
{
    private readonly DocumentAnalysisClient _client;

    public DocumentIntelligenceService()
    {
        // Read the Azure Document Intelligence configuration
        // from environment variables instead of hardcoding secrets.
        var endpoint = Environment.GetEnvironmentVariable("AZURE_DI_ENDPOINT");
        var key = Environment.GetEnvironmentVariable("AZURE_DI_KEY");

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new InvalidOperationException(
                "Environment variable AZURE_DI_ENDPOINT is missing.");
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "Environment variable AZURE_DI_KEY is missing.");
        }

        // Create the Azure Document Intelligence client.
        _client = new DocumentAnalysisClient(
            new Uri(endpoint),
            new AzureKeyCredential(key));
    }

    public async Task<AnalyzeResult> AnalyzeInvoiceAsync(
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
        {
            throw new ArgumentException("Invoice file is empty.", nameof(file));
        }

         // Open the uploaded invoice as a stream.
        await using var stream = file.OpenReadStream();

        // Analyze the invoice using Azure's prebuilt invoice model.
        // WaitUntil.Completed means the method waits until the analysis is finished.
        var operation = await _client.AnalyzeDocumentAsync(
            WaitUntil.Completed,
            "prebuilt-invoice",
            stream,
            cancellationToken: cancellationToken);
         // Return the structured analysis result to the API endpoint.
        return operation.Value;
    }
}