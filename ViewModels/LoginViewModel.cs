using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizApp.Services;

namespace QuizApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthService _auth;
    public LoginViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] string username;
    [ObservableProperty] string password;
    [ObservableProperty] string error;

    [RelayCommand]
    async Task Login()
    {
        if (await _auth.LoginAsync(Username, Password))
        {
            Error = ""; Password = "";
            await Shell.Current.GoToAsync("//categories");
        }
        else Error = "Nom d'utilisateur ou mot de passe incorrect.";
    }

    [RelayCommand]
    async Task GoRegister() => await Shell.Current.GoToAsync("register");
}