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
    /// <summary>
    /// Service class to export journal entries to PDF using Syncfusion PDF library.
    /// </summary>
    public class PdfService
    {
        /// <summary>
        /// Export a list of journal entries to a PDF file.
        /// </summary>
        /// <param name="entries">List of JournalEntry objects to export.</param>
        /// <param name="from">Optional start date for the report.</param>
        /// <param name="to">Optional end date for the report.</param>
        /// <returns>Byte array of the generated PDF document.</returns>
        public byte[] ExportEntriesToPdf(List<JournalEntry> entries, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                // Throw exception if there are no entries to export
                if (entries == null || entries.Count == 0)
                    throw new Exception("No entries to export");

                // Create a new PDF document
                using var document = new PdfDocument();

                // Add first page to the PDF
                var page = document.Pages.Add();

                // Define fonts for title, headers, content, and small details
                var titleFont = new PdfStandardFont(PdfFontFamily.Helvetica, 18, PdfFontStyle.Bold);
                var headerFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
                var contentFont = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Regular);
                var smallFont = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Italic);

                var brush = PdfBrushes.Black; // Brush color for all text and lines

                float y = 20; // Initial vertical position on the page

                // Add title to the PDF
                string title = from.HasValue && to.HasValue
                    ? $"📔 Journal Entries from {from:dd/MM/yyyy} to {to:dd/MM/yyyy}"
                    : "📔 Journal Entries";
                page.Graphics.DrawString(title, titleFont, brush, new PointF(0, y));
                y += 30;

                // Draw a line below the title for visual separation
                page.Graphics.DrawLine(new PdfPen(brush, 1), 0, y, page.GetClientSize().Width, y);
                y += 15;

                // Loop through each journal entry to add details
                foreach (var entry in entries)
                {
                    // Draw entry date
                    page.Graphics.DrawString($"Date: {entry.EntryDate:yyyy-MM-dd}", headerFont, brush, new PointF(0, y));
                    y += 18;

                    // Draw entry title
                    page.Graphics.DrawString($"Title: {entry.Title ?? ""}", contentFont, brush, new PointF(0, y));
                    y += 15;

                    // Draw entry content
                    page.Graphics.DrawString($"Content: {entry.Content ?? ""}", contentFont, brush, new PointF(0, y));
                    y += 15;

                    // Draw entry mood
                    page.Graphics.DrawString($"Mood: {entry.PrimaryMood}", contentFont, brush, new PointF(0, y));
                    y += 15;

                    // Draw entry tags and category in smaller font
                    page.Graphics.DrawString($"Tags: {entry.Tags ?? ""} | Category: {entry.Category ?? ""}", smallFont, brush, new PointF(0, y));
                    y += 15;

                    // Draw horizontal separator line between entries
                    page.Graphics.DrawLine(new PdfPen(brush, 0.5f), 0, y, page.GetClientSize().Width, y);
                    y += 15;

                    // If current page is almost full, add a new page and reset vertical position
                    if (y > page.GetClientSize().Height - 100)
                    {
                        page = document.Pages.Add();
                        y = 20;
                    }
                }

                // Save the PDF to a memory stream and return as byte array
                using var stream = new MemoryStream();
                document.Save(stream);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExportEntriesToPdf: {ex.Message}");
                // Optionally, rethrow to let caller handle it
                throw;
            }
        }
    }
}
