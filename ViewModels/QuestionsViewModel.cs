using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.ViewModels;

[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class QuestionsViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly UserSession _session;
    public ObservableCollection<Question> Questions { get; } = new();

    [ObservableProperty] string categoryId;

    public QuestionsViewModel(DatabaseService db, UserSession session)
    {
        _db = db;
        _session = session;
    }

    public async Task LoadAsync()
    {
        if (string.IsNullOrEmpty(CategoryId)) return;
        Questions.Clear();
        foreach (var q in await _db.GetQuestionsAsync(int.Parse(CategoryId)))
            Questions.Add(q);
    }

    [RelayCommand]
    async Task Add()
    {
        if (!_session.IsAdmin) return;
        await Shell.Current.GoToAsync($"questionedit?categoryId={CategoryId}&questionId=0");
    }

    [RelayCommand]
    async Task Edit(Question q)
    {
        if (!_session.IsAdmin) return;
        await Shell.Current.GoToAsync($"questionedit?categoryId={q.CategoryId}&questionId={q.Id}");
    }

    [RelayCommand]
    async Task Delete(Question q)
    {
        if (!_session.IsAdmin) return;
        bool yes = await Shell.Current.DisplayAlert("Supprimer", "Supprimer cette question ?", "Oui", "Non");
        if (!yes) return;
        await _db.DeleteQuestionAsync(q);
        await LoadAsync();
    }
}