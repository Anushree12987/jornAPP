using SQLite;

namespace jornAPP.Components.Models
{
    // This class represents the relationship between a journal entry and a tag
    // It is used to associate multiple tags with a single journal entry
    public class EntryTag
    {
        // Unique identifier for this EntryTag record
        // Primary key in the SQLite database and auto-incremented
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // The ID of the journal entry this tag is associated with
        public int EntryId { get; set; }

        // The ID of the tag associated with the journal entry
        public int TagId { get; set; }
    }
}