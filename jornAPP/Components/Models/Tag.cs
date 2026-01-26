using SQLite;

namespace jornAPP.Components.Models
{
    // This class represents a Tag in the journaling app
    // Tags are used to categorize or label journal entries
    public class Tag
    {
        // Unique identifier for the tag
        // Primary key in SQLite and auto-incremented
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Name of the tag
        // Must be unique and cannot be null
        [Unique, NotNull]
        public string Name { get; set; } = string.Empty;
    }
}