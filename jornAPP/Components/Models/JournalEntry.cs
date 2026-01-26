using SQLite;

namespace jornAPP.Components.Models
{
    // This class represents a full journal entry in the application
    // Includes title, content, moods, tags, and timestamps
    public class JournalEntry
    {
        // Unique identifier for the journal entry
        // Primary key in SQLite and auto-incremented
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // The ID of the user who created this entry
        // Indexed for faster querying by user
        [Indexed]
        public int UserId { get; set; }

        // The date of the journal entry
        // Must be unique to allow only one entry per day
        [Unique]
        public DateTime EntryDate { get; set; } 

        // Title of the journal entry
        public string Title { get; set; } = string.Empty;

        // Main content of the journal entry
        // Can store rich text
        public string Content { get; set; } = string.Empty;

        // The primary mood selected for this entry
        // Cannot be null
        [NotNull]
        public string PrimaryMood { get; set; } = string.Empty;

        // Optional secondary moods for the entry
        public string? SecondaryMood1 { get; set; }
        public string? SecondaryMood2 { get; set; }

        // Optional comma-separated list of tags for the entry
        public string? Tags { get; set; }

        // Optional category classification: Positive, Neutral, Negative
        public string? Category { get; set; }

        // Timestamp when the entry was created
        public DateTime CreatedAt { get; set; }

        // Timestamp when the entry was last updated
        public DateTime UpdatedAt { get; set; }
    }
}