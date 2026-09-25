namespace Scanly.Api.Models;

public record InvoiceItem(
    string? Description,
    double? Quantity,
    decimal? UnitPrice,
    decimal? Amount
);