using CommunityToolkit.Mvvm.ComponentModel;

namespace QuizApp.ViewModels;

/// <summary>Une réponse affichée dans le quiz (lettre, texte, état visuel).</summary>
public partial class OptionItem : ObservableObject
{
    public int Index { get; init; }
    public string Letter { get; init; }
    public string Text { get; init; }

    [ObservableProperty] bool isVisible = true;     // false après un 50/50
    [ObservableProperty] string state = "idle";     // idle | correct | wrong | dim
    [ObservableProperty] string mark = "";          // ✅ / ❌ après la réponse
}
