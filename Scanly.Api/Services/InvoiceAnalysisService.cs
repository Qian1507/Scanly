using Scanly.Api.Models;

namespace Scanly.Api.Services;

public class InvoiceAnalysisService : IInvoiceAnalysisService
{
    private readonly DocumentIntelligenceService _documentIntelligenceService;

    public InvoiceAnalysisService(
        DocumentIntelligenceService documentIntelligenceService)
    {
        _documentIntelligenceService = documentIntelligenceService;
    }

    public async Task<InvoiceResult> AnalyzeAsync(
        IFormFile file,
        Guid invoiceId,
        CancellationToken cancellationToken = default)
    {
        var analysisResult =
            await _documentIntelligenceService.AnalyzeInvoiceAsync(
                file,
                cancellationToken);

        return InvoiceMapper.Map(
            analysisResult,
            invoiceId,
            file.FileName);
    }
}