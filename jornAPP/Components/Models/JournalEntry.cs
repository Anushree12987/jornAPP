using SQLite;
namespace jornAPP.Components.Models;

public class JournalEntry
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int UserId { get; set; }

    [Unique]
    public DateTime EntryDate { get; set; } // One entry per day

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty; // Supports rich text

    [NotNull]
    public string PrimaryMood { get; set; } = string.Empty;

    public string? SecondaryMood1 { get; set; }
    public string? SecondaryMood2 { get; set; }

    public string? Tags { get; set; } // Comma-separated string of tags
    public string? Category { get; set; } // Positive / Neutral / Negative

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
}
