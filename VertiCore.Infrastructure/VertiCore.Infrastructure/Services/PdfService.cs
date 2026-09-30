using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Domain.Exceptions;
using VertiCore.Infrastructure.Data;

namespace VertiCore.Infrastructure.Services
{
    public class PdfService : IPdfService
    {
        private readonly AppDbContext _context;

        public PdfService(AppDbContext context)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _context = context;
        }

        public byte[] GenerateInvoicePdf(Guid invoiceId, Guid tenantId)
        {
            var invoice = _context.Invoices
                .FirstOrDefault(i => i.Id == invoiceId && i.TenantId == tenantId);

            if (invoice == null)
                throw new InvoiceNotFoundException(invoiceId);

            var client = _context.Clients
                .FirstOrDefault(c => c.Id == invoice.ClientId && c.TenantId == tenantId);

            var tenant = _context.Tenants
                .FirstOrDefault(t => t.Id == tenantId);

            var items = _context.InvoiceItems
                .Where(i => i.InvoiceId == invoiceId)
                .ToList();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(40);
                    page.MarginVertical(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // ── HEADER ──────────────────────────────────────────
                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            // Company name left
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text(tenant?.Name ?? "VertiCore")
                                    .FontSize(22).Bold().FontColor("#1a1a1a");
                                col.Item().Text("Business Management Platform")
                                    .FontSize(9).FontColor("#888888");
                            });

                            // Invoice title right
                            row.ConstantItem(160).AlignRight().Column(col =>
                            {
                                col.Item().Text("INVOICE")
                                    .FontSize(26).Bold().FontColor("#1a1a1a");
                                col.Item().Text(invoice.InvoiceNumber)
                                    .FontSize(11).FontColor("#555555");
                            });
                        });

                        header.Item().PaddingTop(12).LineHorizontal(1.5f).LineColor("#1a1a1a");
                    });

                    // ── CONTENT ─────────────────────────────────────────
                    page.Content().PaddingTop(20).Column(content =>
                    {
                        // Bill To + Invoice Details
                        content.Item().Row(row =>
                        {
                            // Bill To
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("BILL TO").FontSize(8).Bold()
                                    .FontColor("#888888").LetterSpacing(0.1f);
                                col.Item().PaddingTop(4).Text(client?.FullName ?? "N/A")
                                    .FontSize(12).Bold().FontColor("#1a1a1a");
                                if (!string.IsNullOrEmpty(client?.Email))
                                    col.Item().Text(client.Email).FontColor("#555555");
                                if (!string.IsNullOrEmpty(client?.Phone))
                                    col.Item().Text(client.Phone).FontColor("#555555");
                                if (!string.IsNullOrEmpty(client?.Address))
                                    col.Item().Text(client.Address).FontColor("#555555");
                            });

                            // Invoice Details
                            row.ConstantItem(200).Column(col =>
                            {
                                void DetailRow(string label, string value, string color = "#1a1a1a") =>
                                    col.Item().PaddingBottom(4).Row(r =>
                                    {
                                        r.RelativeItem().Text(label).FontColor("#888888");
                                        r.ConstantItem(100).AlignRight()
                                            .Text(value).Bold().FontColor(color);
                                    });

                                DetailRow("Invoice No:", invoice.InvoiceNumber);
                                DetailRow("Issue Date:", invoice.CreatedAt.ToString("dd MMM yyyy"));
                                DetailRow("Due Date:", invoice.DueDate.ToString("dd MMM yyyy"));

                                var statusColor = invoice.Status.ToString() switch
                                {
                                    "Paid" => "#22c55e",
                                    "Overdue" => "#ef4444",
                                    "Sent" => "#3b82f6",
                                    _ => "#f59e0b"
                                };
                                DetailRow("Status:", invoice.Status.ToString(), statusColor);
                            });
                        });

                        content.Item().PaddingTop(24).PaddingBottom(8)
                            .LineHorizontal(0.5f).LineColor("#e5e7eb");

                        // Items Table Header
                        content.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(4);  // Description
                                cols.RelativeColumn(1);  // Qty
                                cols.RelativeColumn(2);  // Unit Price
                                cols.RelativeColumn(2);  // Total
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                void HeaderCell(string text, bool alignRight = false)
                                {
                                    var cell = header.Cell().Background("#1a1a1a").Padding(8);
                                    var txt = cell.Text(text).FontSize(9).Bold().FontColor("#ffffff");
                                    if (alignRight) txt.AlignRight();
                                }

                                HeaderCell("DESCRIPTION");
                                HeaderCell("QTY", true);
                                HeaderCell("UNIT PRICE", true);
                                HeaderCell("TOTAL", true);
                            });

                            // Table Rows
                            if (items.Any())
                            {
                                var rowIndex = 0;
                                foreach (var item in items)
                                {
                                    var bgColor = rowIndex % 2 == 0 ? "#ffffff" : "#f9fafb";
                                    rowIndex++;

                                    void DataCell(string text, bool alignRight = false, bool bold = false)
                                    {
                                        var cell = table.Cell().Background(bgColor).Padding(8);
                                        var txt = cell.Text(text).FontSize(10).FontColor("#374151");
                                        if (alignRight) txt.AlignRight();
                                        if (bold) txt.Bold();
                                    }

                                    DataCell(item.Description);
                                    DataCell(item.Quantity.ToString(), true);
                                    DataCell($"PKR {item.UnitPrice:N2}", true);
                                    DataCell($"PKR {item.Total:N2}", true, true);
                                }
                            }
                            else
                            {
                                table.Cell().ColumnSpan(4).Padding(12)
                                    .Text("No items found").FontColor("#888888").AlignCenter();
                            }
                        });

                        content.Item().PaddingTop(8).LineHorizontal(0.5f).LineColor("#e5e7eb");

                        // Totals
                        content.Item().PaddingTop(8).AlignRight().Column(totals =>
                        {
                            totals.Item().Width(220).Row(row =>
                            {
                                row.RelativeItem().Text("Subtotal").FontColor("#555555");
                                row.ConstantItem(110).AlignRight()
                                    .Text($"PKR {invoice.TotalAmount:N2}").FontColor("#555555");
                            });

                            totals.Item().PaddingTop(4).Width(220)
                                .LineHorizontal(0.5f).LineColor("#e5e7eb");

                            totals.Item().PaddingTop(6).Width(220)
                                .Background("#1a1a1a").Padding(10).Row(row =>
                                {
                                    row.RelativeItem().Text("TOTAL AMOUNT")
                                        .FontSize(11).Bold().FontColor("#ffffff");
                                    row.ConstantItem(110).AlignRight()
                                        .Text($"PKR {invoice.TotalAmount:N2}")
                                        .FontSize(11).Bold().FontColor("#ffffff");
                                });
                        });

                        // Notes
                        if (!string.IsNullOrEmpty(invoice.Notes))
                        {
                            content.Item().PaddingTop(24).Column(notes =>
                            {
                                notes.Item().Text("NOTES").FontSize(8).Bold()
                                    .FontColor("#888888").LetterSpacing(1);
                                notes.Item().PaddingTop(4).Background("#f9fafb")
                                    .Border(0.5f).BorderColor("#e5e7eb")
                                    .Padding(10).Text(invoice.Notes).FontColor("#555555");
                            });
                        }
                    });

                    // ── FOOTER ──────────────────────────────────────────
                    page.Footer().Column(footer =>
                    {
                        footer.Item().LineHorizontal(0.5f).LineColor("#e5e7eb");
                        footer.Item().PaddingTop(8).Row(row =>
                        {
                            row.RelativeItem().Text("Generated by VertiCore")
                                .FontSize(8).FontColor("#aaaaaa");
                            row.RelativeItem().AlignCenter()
                                .Text($"Thank you for your business, {client?.FullName ?? ""}!")
                                .FontSize(8).FontColor("#aaaaaa");
                            row.RelativeItem().AlignRight()
                                .Text($"Page 1 of 1")
                                .FontSize(8).FontColor("#aaaaaa");
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}