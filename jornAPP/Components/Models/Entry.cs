namespace jornAPP.Components.Models
{
    // This class represents a single journal entry in the application
    public class Entry
    {
        // Unique identifier for the journal entry
        public int Id { get; set; }

        // The date and time when the entry was created
        // Defaults to the current date and time if not explicitly set
        public DateTime Date { get; set; } = DateTime.Now;

        // The mood associated with this journal entry
        // Can be something like "Happy", "Sad", "Neutral", etc.
        public string Mood { get; set; }

        // The main content/text of the journal entry
        public string Content { get; set; }
    }
}