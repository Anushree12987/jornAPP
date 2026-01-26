using SQLite;

namespace jornAPP.Components.Models
{
    // This class represents summary information about a tag
    // It can be used to show how many times a specific tag is used in journal entries
    public class TagInfo
    {
        // The name of the tag
        public string Tag { get; set; }

        // The number of times this tag appears in journal entries
        public int Count { get; set; }
    }
}