using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class QuizPage : ContentPage
{
    private readonly QuizViewModel _vm;

    public QuizPage(QuizViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.StartAsync();      // lance la partie une seule fois
    }
}