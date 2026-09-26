using Scanly.Api.Models;
using Scanly.Api.Services;

namespace Scanly.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
        // --------------------------------------------------
        // POST /invoices
        // Upload invoice -> analyze -> save to Blob Storage
        // --------------------------------------------------

        app.MapPost("/invoices", async (
            IFormFile file,
            IInvoiceAnalysisService invoiceAnalysisService,
            IInvoiceStorageService invoiceStorageService,
            CancellationToken cancellationToken) =>
        {
            // Check that a file was uploaded.
            if (file == null || file.Length == 0)
            {
                return Results.BadRequest(new
                {
                    message = "Please upload an invoice file."
                });
            }

            // Generate a unique ID for this invoice.
            var invoiceId = Guid.NewGuid();

            // Analyze the uploaded invoice and map it
            // to the Scanly response model.
            var invoiceResult =
                await invoiceAnalysisService.AnalyzeAsync(
                    file,
                    invoiceId,
                    cancellationToken);

            // Save result as {invoiceId}.json in Azure Blob Storage.
            await invoiceStorageService.SaveAsync(
                invoiceId.ToString(),
                invoiceResult);

            return Results.Ok(invoiceResult);
        })
        .DisableAntiforgery()
        .WithName("UploadInvoice")
        .WithTags("Invoices")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<InvoiceResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);


        // --------------------------------------------------
        // GET /invoices/{id}
        // Retrieve invoice JSON from Blob Storage
        // --------------------------------------------------

        app.MapGet("/invoices/{id}", async (
            Guid id,
            IInvoiceStorageService invoiceStorageService) =>
        {
            var invoice =
                await invoiceStorageService.GetAsync(
                    id.ToString());

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
        .WithTags("Invoices")
        .Produces<InvoiceResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);


        // --------------------------------------------------
        // GET /invoices
        // Retrieve all stored invoices from Blob Storage
        // --------------------------------------------------

        app.MapGet("/invoices", async (
            IInvoiceStorageService invoiceStorageService) =>
        {
            var invoices =
                await invoiceStorageService.GetAllAsync();

            return Results.Ok(invoices);
        })
        .WithName("GetInvoices")
        .WithTags("Invoices")
        .Produces<IReadOnlyList<InvoiceResult>>(
            StatusCodes.Status200OK);
    }
}