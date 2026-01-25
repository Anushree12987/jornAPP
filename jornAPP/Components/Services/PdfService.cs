using System;
using System.Collections.Generic;
using System.IO;
using jornAPP.Components.Models;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Drawing;
using PointF = Syncfusion.Drawing.PointF;

namespace jornAPP.Services
{
    public class PdfService
    {
        public byte[] ExportEntriesToPdf(List<JournalEntry> entries, DateTime? from = null, DateTime? to = null)
        {
            if (entries == null || entries.Count == 0)
                throw new Exception("No entries to export");

            // Create a new PDF document
            using var document = new PdfDocument();

            // Add a page
            var page = document.Pages.Add();

            // Create font and brushes
            var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
            var headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            var contentFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular);
            var smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Italic);

            var brush = PdfBrushes.Black;

            float y = 20; // starting y-coordinate

            // Title
            string title = from.HasValue && to.HasValue
                ? $"📔 Journal Entries from {from:dd/MM/yyyy} to {to:dd/MM/yyyy}"
                : "📔 Journal Entries";
            page.Graphics.DrawString(title, titleFont, brush, new PointF(0, y));
            y += 30;

            // Draw a line below title
            page.Graphics.DrawLine(new PdfPen(brush, 1), 0, y, page.GetClientSize().Width, y);
            y += 15;

            // Loop through entries
            foreach (var entry in entries)
            {
                page.Graphics.DrawString($"Date: {entry.EntryDate:yyyy-MM-dd}", headerFont, brush, new PointF(0, y));
                y += 18;

                page.Graphics.DrawString($"Title: {entry.Title ?? ""}", contentFont, brush, new PointF(0, y));
                y += 15;

                page.Graphics.DrawString($"Content: {entry.Content ?? ""}", contentFont, brush, new PointF(0, y));
                y += 15;

                page.Graphics.DrawString($"Mood: {entry.PrimaryMood}", contentFont, brush, new PointF(0, y));
                y += 15;

                page.Graphics.DrawString($"Tags: {entry.Tags ?? ""} | Category: {entry.Category ?? ""}", smallFont, brush, new PointF(0, y));
                y += 15;

                // Horizontal separator
                page.Graphics.DrawLine(new PdfPen(brush, 0.5f), 0, y, page.GetClientSize().Width, y);
                y += 15;

                // Add new page if we exceed page height
                if (y > page.GetClientSize().Height - 100)
                {
                    page = document.Pages.Add();
                    y = 20;
                }
            }

            // Save to memory stream
            using var stream = new MemoryStream();
            document.Save(stream);
            return stream.ToArray();
        }
    }
}
