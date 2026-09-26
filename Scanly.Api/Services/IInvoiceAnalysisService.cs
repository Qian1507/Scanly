using Scanly.Api.Models;

namespace Scanly.Api.Services;

public interface IInvoiceAnalysisService
{
    Task<InvoiceResult> AnalyzeAsync(
        IFormFile file,
        Guid invoiceId,
        CancellationToken cancellationToken = default);
}