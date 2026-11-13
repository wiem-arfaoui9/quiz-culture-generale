using System.ComponentModel;
using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class QuizPage : ContentPage
{
    private readonly QuizViewModel _vm;

    public QuizPage(QuizViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        vm.PropertyChanged += OnVmPropertyChanged;

        // Responsive : colonne centrée (680 px max) sur grand écran
        SizeChanged += (_, _) =>
        {
            if (Width > 0) content.Padding = new Thickness(Math.Max(16, (Width - 680) / 2), 16);
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.StartAsync();      // lance la partie une seule fois
    }

    // Petites animations déclenchées par les changements du ViewModel
    private async void OnVmPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        try
        {
            switch (e.PropertyName)
            {
                case nameof(QuizViewModel.Current):          // nouvelle question : entrée en douceur
                    await AnimateInAsync(questionCard, 0);
                    await AnimateInAsync(optionsStack, 0);
                    break;

                case nameof(QuizViewModel.Score):            // le score "pop"
                    await scoreBadge.ScaleToAsync(1.25, 130, Easing.CubicOut);
                    await scoreBadge.ScaleToAsync(1, 160, Easing.SpringOut);
                    break;

                case nameof(QuizViewModel.ProgressValue):    // la barre se remplit en douceur
                    AnimateProgress(_vm.ProgressValue);
                    break;

                case nameof(QuizViewModel.IsAnswered):       // la carte de retour glisse vers le haut
                    if (_vm.IsAnswered) await AnimateInAsync(feedbackCard, 30);
                    break;
            }
        }
        catch
        {
            // une animation ne doit jamais faire planter le quiz
        }
    }

    private static async Task AnimateInAsync(VisualElement view, double fromY)
    {
        view.Opacity = 0;
        view.TranslationY = fromY == 0 ? 24 : fromY;
        await Task.WhenAll(
            view.FadeToAsync(1, 320, Easing.CubicOut),
            view.TranslateToAsync(0, 0, 320, Easing.CubicOut));
    }

    private void AnimateProgress(double target)
    {
        double from = progressBar.Progress;
        new Animation(v => progressBar.Progress = v, from, target)
            .Commit(this, "progress", 16, 420, Easing.CubicOut);
    }
}
