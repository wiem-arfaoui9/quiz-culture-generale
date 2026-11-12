using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        // Carte centrée, 440 px maximum (écran large) – pleine largeur sur téléphone
        SizeChanged += (_, _) =>
        {
            if (Width > 0) card.WidthRequest = Math.Min(440, Width - 40);
        };
    }
}