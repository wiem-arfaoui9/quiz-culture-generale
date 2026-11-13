using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizApp.Models;
using QuizApp.Services;

namespace QuizApp.ViewModels;

[QueryProperty(nameof(CategoryId), "categoryId")]
public partial class QuizViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly UserSession _session;

    private List<Question> _questions = new();
    private int _index, _levelCorrect;
    private bool _started, _busy;
    private const int MaxLevel = 3;

    public ObservableCollection<OptionItem> Options { get; } = new();

    [ObservableProperty] string categoryId;
    [ObservableProperty] Question current;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(LevelText))] int level = 1;
    [ObservableProperty] int score;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(HasHints)), NotifyPropertyChangedFor(nameof(CanUseFifty))] int hintsLeft = 3;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(CanUseFifty))] bool isAnswered;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(CanUseFifty))] bool fiftyUsed;
    [ObservableProperty] string progress;
    [ObservableProperty] double progressValue;
    [ObservableProperty] string feedbackKind = "good";      // good | bad
    [ObservableProperty] string feedbackTitle;
    [ObservableProperty] string feedbackText;
    [ObservableProperty] string nextText;

    public string LevelText => $"Niveau {Level}";
    public bool HasHints => HintsLeft > 0;
    public bool CanUseFifty => HintsLeft > 0 && !IsAnswered && !FiftyUsed;

    public QuizViewModel(DatabaseService db, UserSession session)
    {
        _db = db;
        _session = session;
    }

    // Appelé par la page (OnAppearing) : démarre une nouvelle partie
    public async Task StartAsync()
    {
        if (_started || string.IsNullOrEmpty(CategoryId)) return;
        _started = true;
        Level = 1; Score = 0; HintsLeft = 3;
        await LoadLevelAsync();
    }

    async Task LoadLevelAsync()
    {
        _questions = await _db.GetQuestionsAsync(int.Parse(CategoryId), Level);
        _index = 0; _levelCorrect = 0;

        if (_questions.Count == 0)
        {
            if (Level == 1)     // aucune question de niveau 1 dans cette catégorie
            {
                await Shell.Current.DisplayAlert("Quiz",
                    "Cette catégorie n'a pas encore de questions de niveau 1.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else                // plus de questions à ce niveau : le joueur a tout terminé
            {
                Level--;
                await FinishAsync(true);
            }
            return;
        }
        ShowQuestion();
    }

    void ShowQuestion()
    {
        var q = _questions[_index];

        IsAnswered = false;
        FiftyUsed = false;

        Options.Clear();
        var texts = new[] { q.Option1, q.Option2, q.Option3, q.Option4 };
        for (int i = 0; i < texts.Length; i++)
            Options.Add(new OptionItem { Index = i, Letter = "ABCD"[i].ToString(), Text = texts[i] });

        Progress = $"Question {_index + 1} / {_questions.Count}";
        ProgressValue = (double)_index / _questions.Count;
        Current = q;            // en dernier : déclenche l'animation de la page
    }

    // ---------- RÉPONSE + SCORE (retour visuel dans la page, sans boîte de dialogue) ----------
    [RelayCommand]
    void Answer(OptionItem item)
    {
        if (IsAnswered || item == null || Current == null) return;
        IsAnswered = true;

        bool good = item.Index == Current.CorrectIndex;
        if (good)
        {
            Score += 10 * Level;
            _levelCorrect++;
        }

        foreach (var o in Options)
        {
            if (o.Index == Current.CorrectIndex) { o.State = "correct"; o.Mark = "✅"; }
            else if (o == item)                  { o.State = "wrong";   o.Mark = "❌"; }
            else                                 { o.State = "dim";     o.Mark = ""; }
        }

        FeedbackKind = good ? "good" : "bad";
        FeedbackTitle = good ? $"Bravo ! +{10 * Level} points 🎉" : "Oups, ce n'est pas ça 😅";
        FeedbackText = string.IsNullOrWhiteSpace(Current.Explanation)
            ? (good ? "Bonne réponse !" : "Mauvaise réponse.")
            : $"💡 {Current.Explanation}";

        bool last = _index + 1 >= _questions.Count;
        NextText = last ? "Terminer le niveau 🏁" : "Question suivante ➜";
        ProgressValue = (double)(_index + 1) / _questions.Count;
    }

    [RelayCommand]
    async Task Next()
    {
        if (!IsAnswered || _busy) return;
        _busy = true;
        try
        {
            if (_index + 1 < _questions.Count)
            {
                _index++;
                ShowQuestion();
            }
            else await EndOfLevelAsync();
        }
        finally { _busy = false; }
    }

    // ---------- 50/50 (coûte 1 aide, une fois par question) ----------
    [RelayCommand]
    void FiftyFifty()
    {
        if (!CanUseFifty) return;

        var toHide = Options
            .Where(o => o.Index != Current.CorrectIndex)
            .OrderBy(_ => Random.Shared.Next())
            .Take(2).ToList();

        foreach (var o in toHide) o.IsVisible = false;

        HintsLeft--;
        FiftyUsed = true;
    }

    // ---------- FIN D'UN NIVEAU ----------
    async Task EndOfLevelAsync()
    {
        bool passed = _levelCorrect >= Math.Ceiling(_questions.Count * 0.6);

        if (!passed) { await FinishAsync(false); return; }
        if (Level >= MaxLevel) { await FinishAsync(true); return; }

        HintsLeft++;                     // RÉCOMPENSE : +1 aide
        await Shell.Current.DisplayAlert("🎉 Niveau réussi !",
            $"Niveau {Level + 1} débloqué et +1 aide !", "Continuer");
        Level++;
        await LoadLevelAsync();
    }

    // ---------- FIN DE PARTIE ----------
    async Task FinishAsync(bool won)
    {
        await _db.SaveScoreAsync(new ScoreRecord
        {
            Username = _session.Username,
            CategoryId = int.Parse(CategoryId),
            Points = Score,
            LevelReached = Level,
            Date = DateTime.Now
        });

        await Shell.Current.GoToAsync(
            $"result?score={Score}&level={Level}&won={won}&categoryId={CategoryId}");
    }
}
