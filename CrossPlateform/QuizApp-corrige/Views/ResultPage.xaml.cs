using System.ComponentModel;
using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class ResultPage : ContentPage
{
    private readonly ResultViewModel _vm;

    public ResultPage(ResultViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;

        // Carte centrée, 460 px maximum
        SizeChanged += (_, _) =>
        {
            if (Width > 0) card.WidthRequest = Math.Min(460, Width - 48);
        };

        // Le score défile de 0 jusqu'à sa valeur dès qu'il est connu
        vm.PropertyChanged += OnVmPropertyChanged;
    }

    private void OnVmPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ResultViewModel.Score)) return;
        if (!int.TryParse(_vm.Score, out int target)) return;

        new Animation(v => scoreLabel.Text = ((int)Math.Round(v)).ToString(), 0, target)
            .Commit(this, "score", 16, 900, Easing.CubicOut);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        card.Opacity = 0;
        card.TranslationY = 50;
        trophy.Scale = 0.2;

        await Task.WhenAll(
            card.FadeToAsync(1, 450, Easing.CubicOut),
            card.TranslateToAsync(0, 0, 450, Easing.CubicOut));
        await trophy.ScaleToAsync(1, 650, Easing.SpringOut);
    }

    // Le bouton "retour" d'Android ramène à l'accueil (et non au quiz terminé)
    protected override bool OnBackButtonPressed()
    {
        _vm.HomeCommand.Execute(null);
        return true;
    }
}
