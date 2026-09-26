using System.Collections.Concurrent;
using Scanly.Api.Models;
using Scanly.Api.Services;

namespace Scanly.Api.Tests.Fakes;

public class FakeInvoiceStorageService : IInvoiceStorageService
{
    private readonly ConcurrentDictionary<string, InvoiceResult> _storage = new();

    public Task SaveAsync(
        string invoiceId,
        InvoiceResult result)
    {
        _storage[invoiceId] = result;

        return Task.CompletedTask;
    }

    public Task<InvoiceResult?> GetAsync(string invoiceId)
    {
        _storage.TryGetValue(invoiceId, out var result);

        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<InvoiceResult>> GetAllAsync()
    {
        IReadOnlyList<InvoiceResult> results =
            _storage.Values.ToList();

        return Task.FromResult(results);
    }
}