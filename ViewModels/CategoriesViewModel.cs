using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly UserSession _session;
    private Category _editing;

    public ObservableCollection<Category> Categories { get; } = new();

    [ObservableProperty] string name;
    [ObservableProperty] string icon;
    [ObservableProperty] string saveText = "Ajouter";
    [ObservableProperty] string welcome;

    public CategoriesViewModel(DatabaseService db, UserSession session)
    {
        _db = db;
        _session = session;
    }

    public async Task LoadAsync()
    {
        Welcome = $"Bonjour {_session.Username} 👋";
        Categories.Clear();
        foreach (var c in await _db.GetCategoriesAsync())
            Categories.Add(c);
    }

    [RelayCommand]
    async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name)) return;

        if (_editing == null)
            await _db.SaveCategoryAsync(new Category { Name = Name.Trim(), Icon = Icon });
        else
        {
            _editing.Name = Name.Trim();
            _editing.Icon = Icon;
            await _db.SaveCategoryAsync(_editing);
            _editing = null;
        }

        Name = ""; Icon = ""; SaveText = "Ajouter";
        await LoadAsync();
    }

    [RelayCommand]
    void Edit(Category c)
    {
        _editing = c;
        Name = c.Name; Icon = c.Icon;
        SaveText = "Enregistrer";
    }

    [RelayCommand]
    async Task Delete(Category c)
    {
        bool yes = await Shell.Current.DisplayAlert("Supprimer",
            $"Supprimer « {c.Name} » et toutes ses questions ?", "Oui", "Non");
        if (!yes) return;
        await _db.DeleteCategoryAsync(c);
        await LoadAsync();
    }

    [RelayCommand]
    async Task OpenQuestions(Category c)
        => await Shell.Current.GoToAsync($"questions?categoryId={c.Id}");

    [RelayCommand]
    async Task Play(Category c)
        => await Shell.Current.GoToAsync($"quiz?categoryId={c.Id}");

    [RelayCommand]
    async Task Logout()
    {
        _session.Logout();
        await Shell.Current.GoToAsync("//login");
    }
}