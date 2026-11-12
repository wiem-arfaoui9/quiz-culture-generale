using SQLite;

namespace QuizApp.Models;

[Table("Scores")]
public class ScoreRecord
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Username { get; set; }
    public int CategoryId { get; set; }
    public int Points { get; set; }
    public int LevelReached { get; set; }
    public DateTime Date { get; set; }
}