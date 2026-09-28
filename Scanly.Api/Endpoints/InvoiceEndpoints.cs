using Azure;
using Scanly.Api.Models;
using Scanly.Api.Services;

namespace Scanly.Api.Endpoints;

public static class InvoiceEndpoints
{
    private static readonly string[] AllowedExtensions =
    {
        ".pdf",
        ".jpg",
        ".jpeg",
        ".png"
    };

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
            ILogger<Program> logger,
            CancellationToken cancellationToken) =>
        {
            // Check that a file was uploaded.
            if (file == null || file.Length == 0)
            {
                logger.LogWarning(
                    "Invoice upload rejected because the file was empty.");

                return Results.BadRequest(new
                {
                    error = "invalid_file",
                    message = "Please upload a valid invoice file."
                });
            }

            // Check supported file format.
            var extension =
                Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!AllowedExtensions.Contains(extension))
            {
                logger.LogWarning(
                    "Invoice upload rejected because file format {Extension} is not supported.",
                    extension);

                return Results.BadRequest(new
                {
                    error = "unsupported_file_format",
                    message = "Supported formats are PDF, JPG, JPEG and PNG."
                });
            }

            var invoiceId = Guid.NewGuid();

            try
            {
                // Analyze invoice.
                var invoiceResult =
                    await invoiceAnalysisService.AnalyzeAsync(
                        file,
                        invoiceId,
                        cancellationToken);

                // Save result to Blob Storage.
                await invoiceStorageService.SaveAsync(
                    invoiceId.ToString(),
                    invoiceResult);

                logger.LogInformation(
                    "Invoice {InvoiceId} was analyzed and stored successfully.",
                    invoiceId);

                return Results.Ok(invoiceResult);
            }
            catch (RequestFailedException ex) when (ex.Status == 408)
            {
                logger.LogError(
                    ex,
                    "Azure service timed out while processing invoice {InvoiceId}.",
                    invoiceId);

                return Results.Json(
                    new
                    {
                        error = "azure_timeout",
                        message = "The Azure service timed out. Please try again."
                    },
                    statusCode: StatusCodes.Status504GatewayTimeout);
            }
            catch (RequestFailedException ex)
            {
                logger.LogError(
                    ex,
                    "Azure service failed while processing invoice {InvoiceId}. Status: {Status}",
                    invoiceId,
                    ex.Status);

                return Results.Json(
                    new
                    {
                        error = "azure_service_error",
                        message = "An Azure service is temporarily unavailable."
                    },
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (OperationCanceledException ex)
                when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogError(
                    ex,
                    "Azure operation timed out while processing invoice {InvoiceId}.",
                    invoiceId);

                return Results.Json(
                    new
                    {
                        error = "timeout",
                        message = "The Azure operation timed out. Please try again."
                    },
                    statusCode: StatusCodes.Status504GatewayTimeout);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unexpected error while processing invoice {InvoiceId}.",
                    invoiceId);

                return Results.Json(
                    new
                    {
                        error = "internal_server_error",
                        message = "An unexpected error occurred."
                    },
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .DisableAntiforgery()
        .WithName("UploadInvoice")
        .WithTags("Invoices")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<InvoiceResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status503ServiceUnavailable)
        .Produces(StatusCodes.Status504GatewayTimeout);


        // --------------------------------------------------
        // GET /invoices/{id}
        // Retrieve invoice JSON from Blob Storage
        // --------------------------------------------------

        app.MapGet("/invoices/{id}", async (
            Guid id,
            IInvoiceStorageService invoiceStorageService,
            ILogger<Program> logger) =>
        {
            try
            {
                var invoice =
                    await invoiceStorageService.GetAsync(
                        id.ToString());

                if (invoice is null)
                {
                    logger.LogWarning(
                        "Invoice {InvoiceId} was not found.",
                        id);

                    return Results.NotFound(new
                    {
                        error = "invoice_not_found",
                        message = "Invoice not found."
                    });
                }

                return Results.Ok(invoice);
            }
            catch (RequestFailedException ex) when (ex.Status == 408)
            {
                logger.LogError(
                    ex,
                    "Blob Storage timed out while retrieving invoice {InvoiceId}.",
                    id);

                return Results.Json(
                    new
                    {
                        error = "storage_timeout",
                        message = "Blob Storage timed out. Please try again."
                    },
                    statusCode: StatusCodes.Status504GatewayTimeout);
            }
            catch (RequestFailedException ex)
            {
                logger.LogError(
                    ex,
                    "Blob Storage failed while retrieving invoice {InvoiceId}. Status: {Status}",
                    id,
                    ex.Status);

                return Results.Json(
                    new
                    {
                        error = "storage_error",
                        message = "Invoice storage is temporarily unavailable."
                    },
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unexpected error while retrieving invoice {InvoiceId}.",
                    id);

                return Results.Json(
                    new
                    {
                        error = "internal_server_error",
                        message = "An unexpected error occurred."
                    },
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("GetInvoiceById")
        .WithTags("Invoices")
        .Produces<InvoiceResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status503ServiceUnavailable)
        .Produces(StatusCodes.Status504GatewayTimeout);


        // --------------------------------------------------
        // GET /invoices
        // Retrieve all stored invoices from Blob Storage
        // --------------------------------------------------

        app.MapGet("/invoices", async (
            IInvoiceStorageService invoiceStorageService,
            ILogger<Program> logger) =>
        {
            try
            {
                var invoices =
                    await invoiceStorageService.GetAllAsync();

                return Results.Ok(invoices);
            }
            catch (RequestFailedException ex)
            {
                logger.LogError(
                    ex,
                    "Blob Storage failed while retrieving invoices.");

                return Results.Json(
                    new
                    {
                        error = "storage_error",
                        message = "Invoice storage is temporarily unavailable."
                    },
                    statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unexpected error while retrieving invoices.");

                return Results.Json(
                    new
                    {
                        error = "internal_server_error",
                        message = "An unexpected error occurred."
                    },
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        })
        .WithName("GetInvoices")
        .WithTags("Invoices")
        .Produces<IReadOnlyList<InvoiceResult>>(
            StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError)
        .Produces(StatusCodes.Status503ServiceUnavailable);
    }
}