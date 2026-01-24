using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using jornAPP.Components.Data;
using jornAPP.Components.Models;
using QuestPDF.Fluent;

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

        // PDF export
        public (byte[] data, string filename) ExportPdf(DateTime from, DateTime to)
        {
            var entries = GetEntries(from, to);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Header().Text($"Journal Entries from {from:dd/MM/yyyy} to {to:dd/MM/yyyy}").SemiBold().FontSize(16);

                    page.Content().Column(col =>
                    {
                        foreach (var e in entries)
                        {
                            col.Item().Text($"{e.EntryDate:yyyy-MM-dd} - {e.Title} - {e.PrimaryMood}");
                            col.Item().Text(e.Content);
                            col.Item().Text($"Tags: {e.Tags} | Category: {e.Category}").Italic();
                            col.Item().PaddingVertical(5).LineHorizontal(1);
                        }
                    });
                });
            });

            return (doc.GeneratePdf(), $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
        }
    }
}
