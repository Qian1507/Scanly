namespace Scanly.Api.Models;

public record InvoiceResult(
    Guid Id,
    string? FileName,
    string? VendorName,
    string? InvoiceNumber,
    DateTimeOffset? InvoiceDate,
    DateTimeOffset? DueDate,
    decimal? TotalAmount,
    IReadOnlyList<InvoiceItem> Items
);