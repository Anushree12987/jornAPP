using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jornAPP.Components.Data;
using jornAPP.Components.Models;
using SQLite;

namespace jornAPP.Services
{
    // Service class to handle basic CRUD operations for journal entries
    public class JournalService
    {
        private readonly AppDatabase _db; // Reference to the app database

        // Constructor to initialize database
        public JournalService(AppDatabase db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db)); // Ensure database is not null
        }

        /// <summary>
        /// Get all journal entries, ordered by creation date descending (latest first).
        /// </summary>
        /// <returns>List of JournalEntry objects.</returns>
        public async Task<List<JournalEntry>> GetAllEntriesAsync()
        {
            // Query the database and order entries by CreatedAt in descending order
            return await _db.Connection.Table<JournalEntry>()
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get a single journal entry for a specific date.
        /// </summary>
        /// <param name="date">The date for which to retrieve the entry.</param>
        /// <returns>JournalEntry object if found, otherwise null.</returns>
        public async Task<JournalEntry> GetEntryByDateAsync(DateTime date)
        {
            var start = date.Date;          // Start of the day
            var end = start.AddDays(1);     // End of the day (exclusive)

            // Query the database for an entry between start and end dates
            return await _db.Connection.Table<JournalEntry>()
                .Where(e => e.EntryDate >= start && e.EntryDate < end)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Add a new journal entry to the database.
        /// Sets CreatedAt and UpdatedAt timestamps automatically.
        /// </summary>
        /// <param name="entry">JournalEntry object to add.</param>
        public async Task AddEntryAsync(JournalEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            // Set timestamps for new entry
            entry.CreatedAt = DateTime.Now;
            entry.UpdatedAt = DateTime.Now;

            await _db.Connection.InsertAsync(entry); // Insert entry into database
        }

        /// <summary>
        /// Update an existing journal entry.
        /// Updates the UpdatedAt timestamp automatically.
        /// </summary>
        /// <param name="entry">JournalEntry object to update.</param>
        public async Task UpdateEntryAsync(JournalEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            // Update timestamp for modification
            entry.UpdatedAt = DateTime.Now;

            await _db.Connection.UpdateAsync(entry); // Update entry in database
        }

        /// <summary>
        /// Delete a journal entry from the database.
        /// </summary>
        /// <param name="entry">JournalEntry object to delete.</param>
        public async Task DeleteEntryAsync(JournalEntry entry)
        {
            if (entry == null) throw new ArgumentNullException(nameof(entry));

            await _db.Connection.DeleteAsync(entry); // Delete entry from database
        }
    }
}
