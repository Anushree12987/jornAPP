using SQLite;

namespace jornAPP.Components.Models
{
    // This class represents a User in the journaling application
    // Stores login credentials and account creation timestamp
    public class User
    {
        // Unique identifier for the user
        // Primary key in SQLite and auto-incremented
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Username chosen by the user
        // Must be unique and cannot be null
        [Unique, NotNull]
        public string Username { get; set; } = string.Empty;

        // Hashed password for security
        // Cannot be null
        [NotNull]
        public string PasswordHash { get; set; } = string.Empty;

        // Timestamp when the user account was created
        public DateTime CreatedAt { get; set; }
    }
}