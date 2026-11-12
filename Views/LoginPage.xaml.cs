using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm) { InitializeComponent(); BindingContext = vm; }
}