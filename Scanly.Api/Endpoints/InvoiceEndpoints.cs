using Scanly.Api.Services;

namespace Scanly.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
        // --------------------------------------------------
        // POST /invoices
        // Upload invoice -> analyze -> map -> save to Blob
        // --------------------------------------------------
        app.MapPost("/invoices", async (
            IFormFile file,
            DocumentIntelligenceService documentIntelligenceService,
            InvoiceStorageService invoiceStorageService) =>
        {
            // Check that a file was uploaded
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new
                {
                    message = "Please upload an invoice file."
                });
            }

            // Generate unique ID for this invoice
            var invoiceId = Guid.NewGuid();

            var analysisResult =
            await documentIntelligenceService.AnalyzeInvoiceAsync(file);
            // Convert Azure result into our Scanly response model
            var invoiceResult = InvoiceMapper.Map(
                analysisResult,
                invoiceId,
                file.FileName);

            // Save result as {invoiceId}.json in Azure Blob Storage
            await invoiceStorageService.SaveAsync(
                invoiceId.ToString(),
                invoiceResult);

            return Results.Ok(invoiceResult);
        })
        .DisableAntiforgery()
        .WithName("UploadInvoice")
        .WithTags("Invoices");


        // --------------------------------------------------
        // GET /invoices/{id}
        // Retrieve invoice JSON from Azure Blob Storage
        // --------------------------------------------------
        app.MapGet("/invoices/{id}", async (
            Guid id,
            InvoiceStorageService invoiceStorageService) =>
        {
            var invoice =
                await invoiceStorageService.GetAsync(id.ToString());

            if (invoice is null)
            {
                return Results.NotFound(new
                {
                    id,
                    message = "Invoice not found."
                });
            }

            return Results.Ok(invoice);
        })
        .WithName("GetInvoiceById")
        .WithTags("Invoices");


        // --------------------------------------------------
        // GET /invoices
        // Placeholder for listing all invoices
        // --------------------------------------------------
        app.MapGet("/invoices", () =>
        {
            return Results.Ok(new
            {
                message = "Invoice list endpoint."
            });
        })
        .WithName("GetInvoices")
        .WithTags("Invoices");
    }
}