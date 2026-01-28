using System;
using System.IO;
using jornAPP.Components.Models;
using SQLite;
using Microsoft.Maui.Storage;

namespace jornAPP.Components.Data
{
    // This class handles all database operations for the journaling app
    // Uses SQLite for local storage
    public class AppDatabase
    {
        // The main SQLite async connection
        public SQLiteAsyncConnection Connection { get; }

        // Constructor: initializes database and tables
        public AppDatabase()
        {
            // Build the database file path in the app's local storage directory
            string databasePath = Path.Combine(
                FileSystem.AppDataDirectory,  // App-specific storage folder
                "journal_app.db"              // Database file name
            );

            // Initialize SQLite async connection
            Connection = new SQLiteAsyncConnection(databasePath);

            // Create tables if they do not exist
            // Wait() ensures the tables are ready before using the databasee
            Connection.CreateTableAsync<User>().Wait();
            Connection.CreateTableAsync<JournalEntry>().Wait();
            Connection.CreateTableAsync<Tag>().Wait();
            Connection.CreateTableAsync<EntryTag>().Wait();
            Connection.CreateTableAsync<TagInfo>().Wait();
        }

        // ===== Helper Methods ===

        // Insert a new journal entry into the database
        public async Task<int> InsertJournalEntryAsync(JournalEntry entry)
        {
            try
            {
                return await Connection.InsertAsync(entry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting journal entry: {ex.Message}");
                return 0; // indicate failure
            }
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

        // Get all journal entries for a specific user
        // Optional parameters: from and to for date filtering
        public async Task<List<JournalEntry>> GetJournalEntriesAsync(int userId, DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var allEntries = await Connection.Table<JournalEntry>()
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                if (from.HasValue)
                    allEntries = allEntries.Where(x => x.EntryDate.Date >= from.Value.Date).ToList();

                if (to.HasValue)
                    allEntries = allEntries.Where(x => x.EntryDate.Date <= to.Value.Date).ToList();

                return allEntries.OrderByDescending(x => x.EntryDate).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetJournalEntriesAsync: {ex.Message}");
                return new List<JournalEntry>(); // return empty list if error occurs
            }
        }


        // Get a single journal entry for a user by a specific date
        public async Task<JournalEntry> GetEntryByDateAsync(int userId, DateTime date)
        {
            // Fetch all entries for the user
            var allEntries = await Connection.Table<JournalEntry>()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            // Return the entry that matches the given date (or null if not found)
            return allEntries.FirstOrDefault(x => x.EntryDate.Date == date.Date);
        }

        // Get all predefined tags from the database
        public Task<List<Tag>> GetAllTagsAsync()
        {
            return Connection.Table<Tag>().ToListAsync();
        }
    }
}
