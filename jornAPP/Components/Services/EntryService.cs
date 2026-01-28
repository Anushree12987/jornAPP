using jornAPP.Components.Data;
using jornAPP.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jornAPP.Services
{
    // Service class to handle all operations related to Journal Entries
    public class EntryService
    {
        private readonly AppDatabase _db;           // Reference to the app database
        private readonly SecurityService _security; // Reference to security service to get current user

        // Constructor to initialize database and security service
        public EntryService(AppDatabase db, SecurityService security)
        {
            _db = db;
            _security = security;
        }

        /// <summary>
        /// Get all journal entries for the currently logged-in user.
        /// </summary>
        /// <returns>List of JournalEntry objects for current user.</returns>
        public async Task<List<JournalEntry>> GetAllEntriesAsync()
        {
            try
            {
                var user = await _security.GetCurrentUserAsync(); // Get currently logged-in user
                if (user == null) return new List<JournalEntry>(); // If no user is logged in, return empty list

                // Get all entries for the user without any date filters (null = no filter)
                return await _db.GetJournalEntriesAsync(user.Id, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllEntriesAsync: {ex.Message}");
                return new List<JournalEntry>(); // return empty list in case of error
            }
        }

        /// <summary>
        /// Get all journal entries in the database, regardless of user.
        /// Used mainly for export functionality.
        /// </summary>
        /// <returns>List of all JournalEntry objects.</returns>
        public async Task<List<JournalEntry>> GetAllEntriesForExportAsync()
        {
            try
            {
                // Query all entries in the database and order by date descending
                return await _db.Connection.Table<JournalEntry>()
                    .OrderByDescending(e => e.EntryDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllEntriesForExportAsync: {ex.Message}");
                return new List<JournalEntry>(); // return empty list in case of error
            }
        }

        /// <summary>
        /// Delete all orphaned entries where UserId is 0.
        /// Orphaned entries usually occur if an entry was created without a valid user.
        /// </summary>
        public async Task DeleteOrphanedEntriesAsync()
        {
            try
            {
                // Fetch entries where UserId = 0
                var orphanedEntries = await _db.Connection.Table<JournalEntry>()
                    .Where(e => e.UserId == 0)
                    .ToListAsync();

                // Delete each orphaned entry
                foreach (var entry in orphanedEntries)
                {
                    await _db.Connection.DeleteAsync(entry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteOrphanedEntriesAsync: {ex.Message}");
                // Optional: you can rethrow if you want the caller to handle it
            }
        }

        /// <summary>
        /// Add a new journal entry or update an existing one for the current user.
        /// </summary>
        /// <param name="entry">JournalEntry object to add or update.</param>
        public async Task AddOrUpdateEntryAsync(JournalEntry entry)
        {
            try
            {
                var user = await _security.GetCurrentUserAsync(); // Get current user
                if (user == null) throw new Exception("No logged-in user."); // Throw exception if no user

                // Force the entry to belong to the current user
                entry.UserId = user.Id;

                // Check if an entry already exists for this user on the same date
                var existing = await _db.GetEntryByDateAsync(user.Id, entry.EntryDate);
                if (existing != null)
                {
                    // Update existing entry
                    entry.Id = existing.Id;          
                    entry.CreatedAt = existing.CreatedAt; 
                    entry.UpdatedAt = DateTime.Now; 
                    await _db.UpdateJournalEntryAsync(entry);
                }
                else
                {
                    // Insert new entry
                    entry.CreatedAt = DateTime.Now; 
                    entry.UpdatedAt = DateTime.Now; 
                    await _db.InsertJournalEntryAsync(entry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddOrUpdateEntryAsync: {ex.Message}");
                throw; // rethrow to allow caller to handle if needed
            }
        }
    }
}
