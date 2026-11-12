using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.ViewModels;

[QueryProperty(nameof(CategoryId), "categoryId")]
[QueryProperty(nameof(QuestionId), "questionId")]
public partial class QuestionEditViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private Question _question;

    [ObservableProperty] string categoryId;
    [ObservableProperty] string questionId;
    [ObservableProperty] string text;
    [ObservableProperty] string option1;
    [ObservableProperty] string option2;
    [ObservableProperty] string option3;
    [ObservableProperty] string option4;
    [ObservableProperty] int correctIndex = -1;
    [ObservableProperty] int levelIndex = 0;          // 0,1,2 → niveau 1,2,3
    [ObservableProperty] string explanation;
    [ObservableProperty] string error;
    [ObservableProperty] string title = "Nouvelle question";

    public QuestionEditViewModel(DatabaseService db) => _db = db;

    async partial void OnQuestionIdChanged(string value)
    {
        if (value == "0" || string.IsNullOrEmpty(value)) return;

        _question = await _db.GetQuestionAsync(int.Parse(value));
        if (_question == null) return;

        Title = "Modifier la question";
        Text = _question.Text;
        Option1 = _question.Option1; Option2 = _question.Option2;
        Option3 = _question.Option3; Option4 = _question.Option4;
        CorrectIndex = _question.CorrectIndex;
        LevelIndex = _question.Level - 1;
        Explanation = _question.Explanation;
    }

    [RelayCommand]
    async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Text) || string.IsNullOrWhiteSpace(Option1) ||
            string.IsNullOrWhiteSpace(Option2) || string.IsNullOrWhiteSpace(Option3) ||
            string.IsNullOrWhiteSpace(Option4) || CorrectIndex < 0 || LevelIndex < 0)
        {
            Error = "Remplissez tous les champs, la bonne réponse et le niveau.";
            return;
        }

        var q = _question ?? new Question { CategoryId = int.Parse(CategoryId) };
        q.Text = Text.Trim();
        q.Option1 = Option1.Trim(); q.Option2 = Option2.Trim();
        q.Option3 = Option3.Trim(); q.Option4 = Option4.Trim();
        q.CorrectIndex = CorrectIndex;
        q.Level = LevelIndex + 1;
        q.Explanation = Explanation?.Trim();

        await _db.SaveQuestionAsync(q);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    async Task Cancel() => await Shell.Current.GoToAsync("..");
}