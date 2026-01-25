using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using jornAPP.Components.Data;
using jornAPP.Components.Models;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using System.IO;

namespace jornAPP.Services
{
    public class SettingsService
    {
        private readonly AppDatabase _db;

        public SettingsService(AppDatabase db)
        {
            _db = db;
        }

        // Theme and Auto-lock
        public bool IsDarkMode { get; set; } = false;
        public int AutoLockMinutes { get; set; } = 5;

        // Get journal entries
        public List<JournalEntry> GetEntries(DateTime from, DateTime to)
        {
            return _db.Connection.Table<JournalEntry>()
                .Where(x => x.EntryDate.Date >= from.Date && x.EntryDate.Date <= to.Date)
                .ToListAsync().Result;
        }

        // CSV export
        public (byte[] data, string filename) ExportCsv(DateTime from, DateTime to)
        {
            var entries = GetEntries(from, to);
            var sb = new StringBuilder();
            sb.AppendLine("Date,Title,PrimaryMood,SecondaryMood1,SecondaryMood2,Tags,Category,Content");

            foreach (var e in entries)
            {
                sb.AppendLine($"{e.EntryDate:yyyy-MM-dd},{e.Title},{e.PrimaryMood},{e.SecondaryMood1},{e.SecondaryMood2},{e.Tags},{e.Category},{e.Content}");
            }

            return (Encoding.UTF8.GetBytes(sb.ToString()), $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}.csv");
        }

        // PDF export with QuestPDF (works on Mac Catalyst)
        public (byte[] data, string filename) ExportPdf(DateTime from, DateTime to)
        {
            var entries = GetEntries(from, to);
            if (entries == null || entries.Count == 0)
                throw new Exception("No entries to export");

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Journal Entries from {from:dd/MM/yyyy} to {to:dd/MM/yyyy}")
                            .FontSize(18)
                            .Bold()
                            .AlignCenter();

                        col.Item().LineHorizontal(1);

                        foreach (var e in entries)
                        {
                            col.Item().Text($"Date: {e.EntryDate:yyyy-MM-dd}").FontSize(12).Bold();
                            col.Item().Text($"Title: {e.Title ?? ""}").FontSize(12);
                            col.Item().Text($"Content: {e.Content ?? ""}").FontSize(12);
                            col.Item().Text($"Tags: {e.Tags ?? ""} | Category: {e.Category ?? ""}")
                                .FontSize(10)
                                .Italic();
                            col.Item().LineHorizontal(0.5f);
                        }
                    });
                });
            }).GeneratePdf();

            return (pdfBytes, $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
        }
    }
}
