namespace Scanly.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
        // POST
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

            return Results.Ok(new
            {
                id = invoiceId,
                fileName = file.FileName,
                message = "Invoice uploaded successfully."
            });
        })
        .DisableAntiforgery()
        .WithName("UploadInvoice")
        .WithOpenApi();

        // GET invoice by ID
        app.MapGet("/invoices/{id}", (Guid id) =>
        {
            return Results.Ok(new
            {
                id = id,
                message = "Invoice found."
            });
        })
        .WithName("GetInvoiceById")
        .WithTags("Invoices")
        .WithOpenApi();

        // GET /invoices
        app.MapGet("/invoices", () =>
        {
            return Results.Ok(new[]
            {
        new
        {
            message = "Invoice list will be returned here."
        }
    });
        })
        .WithName("GetInvoices")
        .WithTags("Invoices")
        .WithOpenApi();
    }
}
