using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        SizeChanged += (_, _) =>
        {
            if (Width > 0) card.WidthRequest = Math.Min(440, Width - 40);
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        logo.Scale = 0.3;
        card.Opacity = 0;
        card.TranslationY = 40;

        await Task.WhenAll(
            logo.ScaleToAsync(1, 600, Easing.SpringOut),
            card.FadeToAsync(1, 450, Easing.CubicOut),
            card.TranslateToAsync(0, 0, 450, Easing.CubicOut));
    }
}
