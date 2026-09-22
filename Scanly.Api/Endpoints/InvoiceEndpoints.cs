namespace Scanly.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static void MapInvoiceEndpoints(this WebApplication app)
    {
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
    }
}