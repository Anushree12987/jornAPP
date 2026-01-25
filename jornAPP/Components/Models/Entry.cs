namespace jornAPP.Components.Models;

public class Entry
{
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Mood { get; set; }
    public string Content { get; set; }
}