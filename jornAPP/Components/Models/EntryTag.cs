using SQLite;
namespace jornAPP.Components.Models;

public class EntryTag
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int EntryId { get; set; }
    public int TagId { get; set; }
}