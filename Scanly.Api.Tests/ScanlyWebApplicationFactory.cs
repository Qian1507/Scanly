using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scanly.Api.Services;
using Scanly.Api.Tests.Fakes;

namespace Scanly.Api.Tests;

public class ScanlyWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IInvoiceAnalysisService>();
            services.RemoveAll<IInvoiceStorageService>();

            services.AddSingleton<
                IInvoiceAnalysisService,
                FakeInvoiceAnalysisService>();

            services.AddSingleton<
                IInvoiceStorageService,
                FakeInvoiceStorageService>();
        });
    }
}