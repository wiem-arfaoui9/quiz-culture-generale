using Microsoft.Maui.Controls;
using SQLite;

namespace QuizApp.Models;

[Table("Categories")]
public class Category
{
    [PrimaryKey, AutoIncrement] public int Id { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }

    // Affichage seulement (non enregistré dans la base)
    [Ignore] public string BestText { get; set; }
    [Ignore] public Brush Banner { get; set; }
}
