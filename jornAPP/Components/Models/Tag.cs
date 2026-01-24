using SQLite;
namespace jornAPP.Components.Models;

public class Tag
{
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

    [Unique, NotNull]
    public string Name { get; set; } = string.Empty;
}