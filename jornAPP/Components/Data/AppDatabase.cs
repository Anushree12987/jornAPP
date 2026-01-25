using System;
using System.IO;
using jornAPP.Components.Models;
using SQLite;
using Microsoft.Maui.Storage;
namespace jornAPP.Components.Data;

public class AppDatabase
{
        public SQLiteAsyncConnection Connection { get; }

        public AppDatabase()
        {
            string databasePath = Path.Combine(
                FileSystem.AppDataDirectory,
                "journal_app.db"
            );

            Connection = new SQLiteAsyncConnection(databasePath);

            // Create tables
            Connection.CreateTableAsync<User>().Wait();
            Connection.CreateTableAsync<JournalEntry>().Wait();
            Connection.CreateTableAsync<Tag>().Wait();
            Connection.CreateTableAsync<EntryTag>().Wait();
            
        }
        // ===== Helper Methods =====

        // Insert a new journal entry
        public Task<int> InsertJournalEntryAsync(JournalEntry entry)
        {
            return Connection.InsertAsync(entry);
        }

        // Update an existing journal entry
        public Task<int> UpdateJournalEntryAsync(JournalEntry entry)
        {
            return Connection.UpdateAsync(entry);
        }

        // Delete a journal entry
        public Task<int> DeleteJournalEntryAsync(JournalEntry entry)
        {
            return Connection.DeleteAsync(entry);
        }

        // Get all entries for a user (optionally by date range)
        // Get all entries for a user (optionally by date range)
        public async Task<List<JournalEntry>> GetJournalEntriesAsync(int userId, DateTime? from = null, DateTime? to = null)
        {
            // Get all entries for the user first
            var allEntries = await Connection.Table<JournalEntry>()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            // Filter by date in memory (after retrieving from database)
            if (from.HasValue)
                allEntries = allEntries.Where(x => x.EntryDate.Date >= from.Value.Date).ToList();

            if (to.HasValue)
                allEntries = allEntries.Where(x => x.EntryDate.Date <= to.Value.Date).ToList();

            return allEntries.OrderByDescending(x => x.EntryDate).ToList();
        }

// Get a single entry by date
        public async Task<JournalEntry> GetEntryByDateAsync(int userId, DateTime date)
        {
            var allEntries = await Connection.Table<JournalEntry>()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return allEntries.FirstOrDefault(x => x.EntryDate.Date == date.Date);
        }

        // Get all tags
        public Task<List<Tag>> GetAllTagsAsync()
        {
            return Connection.Table<Tag>().ToListAsync();
        }
}
    
