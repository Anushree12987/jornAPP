using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jornAPP.Components.Data;
using jornAPP.Components.Models;
using SQLite;

namespace jornAPP.Services
{
    public class JournalService
    {
        private readonly AppDatabase _db;

        public JournalService(AppDatabase db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // Get all journal entries (latest first)
        public async Task<List<JournalEntry>> GetAllEntriesAsync()
        {
            return await _db.Connection.Table<JournalEntry>()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        // Get a single entry by date
        public async Task<JournalEntry> GetEntryByDateAsync(DateTime date)
        {
            var start = date.Date;
            var end = start.AddDays(1);

            return await _db.Connection.Table<JournalEntry>()
                .Where(e => e.EntryDate >= start && e.EntryDate < end)
                .FirstOrDefaultAsync();
        }

        // Add a new journal entry
        public async Task AddEntryAsync(JournalEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            entry.CreatedAt = DateTime.Now;
            entry.UpdatedAt = DateTime.Now;

            await _db.Connection.InsertAsync(entry);
        }

        // Update an existing journal entry
        public async Task UpdateEntryAsync(JournalEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            entry.UpdatedAt = DateTime.Now;
            await _db.Connection.UpdateAsync(entry);
        }

        // Delete a journal entry
        public async Task DeleteEntryAsync(JournalEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            await _db.Connection.DeleteAsync(entry);
        }
    }
}