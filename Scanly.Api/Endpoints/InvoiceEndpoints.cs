namespace Scanly.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
        // POST /invoices
        app.MapPost("/invoices", async (IFormFile file) =>
        {
            if (file.Length == 0)
            {
                return Results.BadRequest(new
                {
                    message = "No invoice file was uploaded."
                });
            }

            var invoiceId = Guid.NewGuid();

            // Later:
            // await documentIntelligenceService.AnalyzeInvoiceAsync(...);
            // await blobStorageService.SaveInvoiceAsync(...);

            return Results.Ok(new
            {
                id = invoiceId,
                fileName = file.FileName,
                message = "Invoice uploaded successfully."
            });
        })
        .DisableAntiforgery()
        .WithName("UploadInvoice")
        .WithTags("Invoices");


        // GET /invoices/{id}
        app.MapGet("/invoices/{id}", async (Guid id) =>
        {
            // Later:
            // var invoice = await blobStorageService.GetInvoiceAsync(id);

            return Results.Ok(new
            {
                id = id,
                message = "Invoice found."
            });
        })
        .WithName("GetInvoiceById")
        .WithTags("Invoices");


        // GET /invoices
        app.MapGet("/invoices", async () =>
        {
            // Later:
            // var invoices = await blobStorageService.GetInvoicesAsync();

            return Results.Ok(new[]
            {
                new
                {
                    message = "Invoice list will be returned here."
                }
            });
        })
        .WithName("GetInvoices")
        .WithTags("Invoices");
    }
}    
