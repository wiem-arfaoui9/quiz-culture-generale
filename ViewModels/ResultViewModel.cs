using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizApp.Services;

namespace QuizApp.ViewModels;

[QueryProperty(nameof(Score), "score")]
[QueryProperty(nameof(Level), "level")]
[QueryProperty(nameof(Won), "won")]
[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class ResultViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly UserSession _session;

    [ObservableProperty] string score;
    [ObservableProperty] string level;
    [ObservableProperty] string won;
    [ObservableProperty] string categoryId;
    [ObservableProperty] string title;
    [ObservableProperty] string best;

    public ResultViewModel(DatabaseService db, UserSession session)
    {
        _db = db;
        _session = session;
    }

    partial void OnWonChanged(string value)
        => Title = value == "True" ? "🏆 Bravo, quiz terminé !" : "😕 Niveau non réussi";

    async partial void OnCategoryIdChanged(string value)
    {
        int b = await _db.GetBestScoreAsync(_session.Username, int.Parse(value));
        Best = $"Meilleur score : {b} pts";
    }

    [RelayCommand]
    async Task Replay()
    {
        var id = CategoryId;
        await Shell.Current.GoToAsync("//categories");
        await Shell.Current.GoToAsync($"quiz?categoryId={id}");
    }

    [RelayCommand]
    async Task Home() => await Shell.Current.GoToAsync("//categories");
}