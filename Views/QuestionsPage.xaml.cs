using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class QuestionsPage : ContentPage
{
    private readonly QuestionsViewModel _vm;

    public QuestionsPage(QuestionsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}