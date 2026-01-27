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
    /// <summary>
    /// Handles user settings (theme, auto-lock) and export functionalities (CSV, PDF) for journal entries.
    /// </summary>
    public class SettingsService
    {
        private readonly AppDatabase _db; // Reference to the app database

        /// <summary>
        /// Constructor to initialize database dependency.
        /// </summary>
        public SettingsService(AppDatabase db)
        {
            _db = db;
        }

        // === USER SETTINGS ===

        /// <summary>
        /// True if dark mode is enabled, false otherwise.
        /// </summary>
        public bool IsDarkMode { get; set; } = false;

        /// <summary>
        /// Auto-lock timer in minutes.
        /// </summary>
        public int AutoLockMinutes { get; set; } = 5;

        // === JOURNAL ENTRY RETRIEVAL ===

        /// <summary>
        /// Get all journal entries in a specified date range.
        /// </summary>
        /// <param name="from">Start date.</param>
        /// <param name="to">End date.</param>
        /// <returns>List of JournalEntry objects.</returns>
        public List<JournalEntry> GetEntries(DateTime from, DateTime to)
        {
            // Query entries from the database where EntryDate is within range
            return _db.Connection.Table<JournalEntry>()
                .Where(x => x.EntryDate.Date >= from.Date && x.EntryDate.Date <= to.Date)
                .ToListAsync().Result; // Note: synchronous wait
        }

        // === CSV EXPORT ===

        /// <summary>
        /// Export journal entries to CSV format.
        /// </summary>
        /// <param name="from">Start date for export.</param>
        /// <param name="to">End date for export.</param>
        /// <returns>Tuple containing CSV data bytes and filename.</returns>
        public (byte[] data, string filename) ExportCsv(DateTime from, DateTime to)
        {
            var entries = GetEntries(from, to);

            // Create CSV header
            var sb = new StringBuilder();
            sb.AppendLine("Date,Title,PrimaryMood,SecondaryMood1,SecondaryMood2,Tags,Category,Content");

            // Add each entry as a CSV line
            foreach (var e in entries)
            {
                sb.AppendLine($"{e.EntryDate:yyyy-MM-dd},{e.Title},{e.PrimaryMood},{e.SecondaryMood1},{e.SecondaryMood2},{e.Tags},{e.Category},{e.Content}");
            }

            // Return CSV as UTF-8 bytes and a filename with date range
            return (Encoding.UTF8.GetBytes(sb.ToString()), $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}.csv");
        }

        // === PDF EXPORT USING QuestPDF ===

        /// <summary>
        /// Export journal entries to PDF format using QuestPDF.
        /// </summary>
        /// <param name="from">Start date for export.</param>
        /// <param name="to">End date for export.</param>
        /// <returns>Tuple containing PDF data bytes and filename.</returns>
        public (byte[] data, string filename) ExportPdf(DateTime from, DateTime to)
        {
            var entries = GetEntries(from, to);

            if (entries == null || entries.Count == 0)
                throw new Exception("No entries to export");

            // Generate PDF using QuestPDF fluent API
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.Content().Column(col =>
                    {
                        // Title
                        col.Item().Text($"Journal Entries from {from:dd/MM/yyyy} to {to:dd/MM/yyyy}")
                            .FontSize(18)
                            .Bold()
                            .AlignCenter();

                        col.Item().LineHorizontal(1); // Horizontal line

                        // Loop through entries
                        foreach (var e in entries)
                        {
                            col.Item().Text($"Date: {e.EntryDate:yyyy-MM-dd}").FontSize(12).Bold();
                            col.Item().Text($"Title: {e.Title ?? ""}").FontSize(12);
                            col.Item().Text($"Content: {e.Content ?? ""}").FontSize(12);
                            col.Item().Text($"Tags: {e.Tags ?? ""} | Category: {e.Category ?? ""}")
                                .FontSize(10)
                                .Italic();
                            col.Item().LineHorizontal(0.5f); // Separator line
                        }
                    });
                });
            }).GeneratePdf();

            // Return PDF bytes and filename with date range
            return (pdfBytes, $"Journal_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
        }
    }
}
