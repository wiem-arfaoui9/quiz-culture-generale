using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using QuizApp.Services;

namespace QuizApp.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly AuthService _auth;
    public RegisterViewModel(AuthService auth) => _auth = auth;

    [ObservableProperty] string username;
    [ObservableProperty] string password;
    [ObservableProperty, NotifyPropertyChangedFor(nameof(HasError))] string error;

    public bool HasError => !string.IsNullOrWhiteSpace(Error);

    [RelayCommand]
    async Task Register()
    {
        var (ok, msg) = await _auth.RegisterAsync(Username, Password);
        if (ok)
        {
            Error = "";
            await Shell.Current.DisplayAlert("Succès", msg, "OK");
            await Shell.Current.GoToAsync("..");
        }
        else Error = msg;
    }

    [RelayCommand]
    async Task Back() => await Shell.Current.GoToAsync("..");
}
