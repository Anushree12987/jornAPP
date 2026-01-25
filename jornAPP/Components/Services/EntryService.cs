using jornAPP.Components.Data;
using jornAPP.Components.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jornAPP.Services
{
    public class EntryService
    {
        private readonly AppDatabase _db;
        private readonly SecurityService _security;

        public EntryService(AppDatabase db, SecurityService security)
        {
            _db = db;
            _security = security;
        }

        // Get all entries for current user
        public async Task<List<JournalEntry>> GetAllEntriesAsync()
        {
            var user = await _security.GetCurrentUserAsync();
            if (user == null) return new List<JournalEntry>();
            return await _db.GetJournalEntriesAsync(user.Id, null, null); // Pass null for no date filter
        }

        // Get entries in date range
        // Get ALL entries regardless of user (for export)
        // Get ALL entries in database (for export)
        public async Task<List<JournalEntry>> GetAllEntriesForExportAsync()
        {
            return await _db.Connection.Table<JournalEntry>()
                .OrderByDescending(e => e.EntryDate)
                .ToListAsync();
        }
        // Delete all entries with UserId = 0
        public async Task DeleteOrphanedEntriesAsync()
        {
            var orphanedEntries = await _db.Connection.Table<JournalEntry>()
                .Where(e => e.UserId == 0)
                .ToListAsync();
    
            foreach (var entry in orphanedEntries)
            {
                await _db.Connection.DeleteAsync(entry);
            }
        }
        
        

        public async Task AddOrUpdateEntryAsync(JournalEntry entry)
        {
            var user = await _security.GetCurrentUserAsync();
            if (user == null) throw new Exception("No logged-in user.");
    
            // FORCE the UserId to the current user
            entry.UserId = user.Id;

            var existing = await _db.GetEntryByDateAsync(user.Id, entry.EntryDate);
            if (existing != null)
            {
                entry.Id = existing.Id;
                entry.CreatedAt = existing.CreatedAt;
                entry.UpdatedAt = DateTime.Now;
                await _db.UpdateJournalEntryAsync(entry);
            }
            else
            {
                entry.CreatedAt = DateTime.Now;
                entry.UpdatedAt = DateTime.Now;
                await _db.InsertJournalEntryAsync(entry);
            }
        }
    }
}