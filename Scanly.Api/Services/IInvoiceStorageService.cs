using Scanly.Api.Models;

namespace Scanly.Api.Services;

public interface IInvoiceStorageService
{
    Task SaveAsync(string invoiceId, InvoiceResult result);

    Task<InvoiceResult?> GetAsync(string invoiceId);

    Task<IReadOnlyList<InvoiceResult>> GetAllAsync();
}