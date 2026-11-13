using SQLite;

namespace QuizApp.Models;

[Table("Questions")]
public class Question
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public int CategoryId { get; set; }
    public int Level { get; set; } = 1;              // NOUVEAU : 1, 2 ou 3
    public string Text { get; set; }
    public string Option1 { get; set; }
    public string Option2 { get; set; }
    public string Option3 { get; set; }
    public string Option4 { get; set; }
    public int CorrectIndex { get; set; }
    public string Explanation { get; set; }          // NOUVEAU : optionnel

    [Ignore] public string LevelText => $"Niveau {Level}";   // affichage seulement
}