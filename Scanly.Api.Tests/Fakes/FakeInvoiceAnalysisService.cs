using Scanly.Api.Models;
using Scanly.Api.Services;
using Microsoft.AspNetCore.Http;
namespace Scanly.Api.Tests.Fakes;

public class FakeInvoiceAnalysisService : IInvoiceAnalysisService
{
    public Task<InvoiceResult> AnalyzeAsync(
        IFormFile file,
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var result = new InvoiceResult(
            invoiceId,
            file.FileName,
            "Test Vendor",
            "INV-TEST-001",
            DateTimeOffset.Parse("2026-01-01"),
            DateTimeOffset.Parse("2026-01-31"),
            1000m,
            new List<InvoiceItem>()
        );

        return Task.FromResult(result);
    }
}