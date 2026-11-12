using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class ResultPage : ContentPage
{
    public ResultPage(ResultViewModel vm) { InitializeComponent(); BindingContext = vm; }
}