using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizApp.Helpers;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly UserSession _session;
    private Category _editing;

    public ObservableCollection<Category> Categories { get; } = new();

    // Emojis proposés à l'admin pour choisir l'icône d'une catégorie
    public string[] IconChoices { get; } = { "🌍", "💻", "🎬", "⚽", "🎨", "🔬", "📚", "🎵", "🍕", "🚀" };

    [ObservableProperty] string name;
    [ObservableProperty] string icon;
    [ObservableProperty] string saveText = "Ajouter";
    [ObservableProperty] string welcome;
    [ObservableProperty] bool isAdmin;
    [ObservableProperty] string roleText;

    public CategoriesViewModel(DatabaseService db, UserSession session)
    {
        _db = db;
        _session = session;
    }

    public async Task LoadAsync()
    {
        Welcome = $"Salut {_session.Username} 👋";
        IsAdmin = _session.IsAdmin;
        RoleText = IsAdmin
            ? "Mode administrateur : gérez les catégories et les questions."
            : "Choisissez une catégorie pour passer le test.";
        Categories.Clear();
        int i = 0;
        foreach (var c in await _db.GetCategoriesAsync())
        {
            c.Banner = Palette.Banner(i++);
            int best = await _db.GetBestScoreAsync(_session.Username, c.Id);
            c.BestText = best > 0 ? $"🏆 Meilleur score : {best} pts" : "Pas encore joué";
            Categories.Add(c);
        }
    }

    [RelayCommand]
    async Task Save()
    {
        if (!_session.IsAdmin || string.IsNullOrWhiteSpace(Name)) return;

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
    void PickIcon(string emoji) => Icon = emoji;

    [RelayCommand]
    void Edit(Category c)
    {
        if (!_session.IsAdmin) return;
        _editing = c;
        Name = c.Name; Icon = c.Icon;
        SaveText = "Enregistrer";
    }

    [RelayCommand]
    async Task Delete(Category c)
    {
        if (!_session.IsAdmin) return;
        bool yes = await Shell.Current.DisplayAlert("Supprimer",
            $"Supprimer « {c.Name} » et toutes ses questions ?", "Oui", "Non");
        if (!yes) return;
        await _db.DeleteCategoryAsync(c);
        await LoadAsync();
    }

    [RelayCommand]
    async Task OpenQuestions(Category c)
    {
        if (!_session.IsAdmin) return;
        await Shell.Current.GoToAsync($"questions?categoryId={c.Id}");
    }

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