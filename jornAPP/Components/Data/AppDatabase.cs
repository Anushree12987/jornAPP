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
        public async Task<List<JournalEntry>> GetJournalEntriesAsync(int userId, DateTime? from = null, DateTime? to = null)
        {
            var query = Connection.Table<JournalEntry>().Where(x => x.UserId == userId);

            if (from.HasValue)
                query = query.Where(x => x.EntryDate.Date >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(x => x.EntryDate.Date <= to.Value.Date);

            return await query.ToListAsync();
        }

        // Get a single entry by date
        public Task<JournalEntry> GetEntryByDateAsync(int userId, DateTime date)
        {
            return Connection.Table<JournalEntry>()
                .Where(x => x.UserId == userId && x.EntryDate.Date == date.Date)
                .FirstOrDefaultAsync();
        }

        // Get all tags
        public Task<List<Tag>> GetAllTagsAsync()
        {
            return Connection.Table<Tag>().ToListAsync();
        }
}
    
