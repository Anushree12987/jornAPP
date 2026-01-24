using SQLite;
namespace jornAPP.Components.Models;

    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Unique, NotNull]
        public string Username { get; set; } = string.Empty;

        [NotNull]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
