using Azure.AI.FormRecognizer.DocumentAnalysis;
using Scanly.Api.Models;

namespace Scanly.Api.Services;

public static class InvoiceMapper
{
    // Converts the large Azure Document Intelligence AnalyzeResult
    // into the smaller Scanly API response model.
    public static InvoiceResult Map(
        AnalyzeResult analysisResult,
        Guid invoiceId,
        string fileName)
    {
        // The prebuilt-invoice model normally returns one analyzed document.
        var document = analysisResult.Documents.FirstOrDefault();

        // If Azure could not identify an invoice document,
        // return an empty Scanly result instead of failing the whole request.
        if (document is null)
        {
            return new InvoiceResult(
                invoiceId,
                fileName,
                null,
                null,
                null,
                null,
                null,
                []);
        }

        var fields = document.Fields;

        // Extract simple text fields from the Azure response.
        var vendorName = fields.TryGetValue("VendorName", out var vendorField)
            ? vendorField.Content
            : null;

        var invoiceNumber = fields.TryGetValue("InvoiceId", out var invoiceIdField)
            ? invoiceIdField.Content
            : null;

        // Azure returns invoice dates as DateTimeOffset.
        DateTimeOffset? invoiceDate = null;
        if (fields.TryGetValue("InvoiceDate", out var invoiceDateField) &&
            invoiceDateField.FieldType == DocumentFieldType.Date)
        {
            invoiceDate = invoiceDateField.Value.AsDate();
        }

        DateTimeOffset? dueDate = null;
        if (fields.TryGetValue("DueDate", out var dueDateField) &&
            dueDateField.FieldType == DocumentFieldType.Date)
        {
            dueDate = dueDateField.Value.AsDate();
        }

        // Currency values from Azure are converted to decimal
        // because decimal is more suitable for monetary values.
        decimal? totalAmount = null;
        if (fields.TryGetValue("InvoiceTotal", out var totalField) &&
            totalField.FieldType == DocumentFieldType.Currency)
        {
            totalAmount = (decimal)totalField.Value.AsCurrency().Amount;
        }

        var items = new List<InvoiceItem>();

        // Map each line item from the Azure invoice result
        // to the smaller Scanly InvoiceItem DTO.
        if (fields.TryGetValue("Items", out var itemsField) &&
            itemsField.FieldType == DocumentFieldType.List)
        {
            foreach (var item in itemsField.Value.AsList())
            {
                var itemFields = item.Value.AsDictionary();

                string? description =
                    itemFields.TryGetValue("Description", out var descriptionField)
                        ? descriptionField.Content
                        : null;

                double? quantity = null;
                if (itemFields.TryGetValue("Quantity", out var quantityField) &&
                    quantityField.FieldType == DocumentFieldType.Double)
                {
                    quantity = quantityField.Value.AsDouble();
                }

                decimal? unitPrice = null;
                if (itemFields.TryGetValue("UnitPrice", out var unitPriceField) &&
                    unitPriceField.FieldType == DocumentFieldType.Currency)
                {
                    unitPrice =
                        (decimal)unitPriceField.Value.AsCurrency().Amount;
                }

                decimal? amount = null;
                if (itemFields.TryGetValue("Amount", out var amountField) &&
                    amountField.FieldType == DocumentFieldType.Currency)
                {
                    amount =
                        (decimal)amountField.Value.AsCurrency().Amount;
                }

                items.Add(new InvoiceItem(
                    description,
                    quantity,
                    unitPrice,
                    amount));
            }
        }

        // Return only the fields that Scanly needs.
        return new InvoiceResult(
            invoiceId,
            fileName,
            vendorName,
            invoiceNumber,
            invoiceDate,
            dueDate,
            totalAmount,
            items);
    }
}