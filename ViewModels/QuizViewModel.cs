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
    private bool _fiftyUsed, _started, _answering;
    private const int MaxLevel = 3;

    [ObservableProperty] string categoryId;
    [ObservableProperty] Question current;
    [ObservableProperty] int level = 1;
    [ObservableProperty] int score;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(HasHints))] int hintsLeft = 3;
    [ObservableProperty] string progress;
    [ObservableProperty] bool o1Visible = true;
    [ObservableProperty] bool o2Visible = true;
    [ObservableProperty] bool o3Visible = true;
    [ObservableProperty] bool o4Visible = true;

    public bool HasHints => HintsLeft > 0;

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
            if (Level == 1)
            {
                await Shell.Current.DisplayAlert("Quiz",
                    "Cette catégorie n'a pas encore de questions de niveau 1.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
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
        Current = _questions[_index];
        _fiftyUsed = false;
        O1Visible = O2Visible = O3Visible = O4Visible = true;
        Progress = $"Niveau {Level} · Question {_index + 1}/{_questions.Count}";
    }

    [RelayCommand]
    async Task Answer(string choice)
    {
        if (_answering) return;
        _answering = true;

        bool good = int.Parse(choice) == Current.CorrectIndex;
        if (good)
        {
            Score += 10 * Level;
            _levelCorrect++;
        }

        string message = good ? $"Bonne réponse ! +{10 * Level} points" : "Mauvaise réponse.";
        if (!string.IsNullOrWhiteSpace(Current.Explanation))
            message += $"\n\n💡 {Current.Explanation}";

        await Shell.Current.DisplayAlert(good ? "✅ Bravo !" : "❌ Faux", message, "Suivant");

        if (_index + 1 < _questions.Count)
        {
            _index++;
            ShowQuestion();
        }
        else await EndOfLevelAsync();

        _answering = false;
    }

    [RelayCommand]
    void FiftyFifty()
    {
        if (HintsLeft <= 0 || _fiftyUsed) return;

        var wrong = Enumerable.Range(0, 4)
            .Where(i => i != Current.CorrectIndex)
            .OrderBy(_ => Random.Shared.Next())
            .Take(2).ToList();

        O1Visible = !wrong.Contains(0);
        O2Visible = !wrong.Contains(1);
        O3Visible = !wrong.Contains(2);
        O4Visible = !wrong.Contains(3);

        HintsLeft--;
        _fiftyUsed = true;
    }

    async Task EndOfLevelAsync()
    {
        bool passed = _levelCorrect >= Math.Ceiling(_questions.Count * 0.6);

        if (!passed) { await FinishAsync(false); return; }
        if (Level >= MaxLevel) { await FinishAsync(true); return; }

        HintsLeft++;
        await Shell.Current.DisplayAlert("🎉 Niveau réussi !",
            $"Niveau {Level + 1} débloqué et +1 aide !", "Continuer");
        Level++;
        await LoadLevelAsync();
    }

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