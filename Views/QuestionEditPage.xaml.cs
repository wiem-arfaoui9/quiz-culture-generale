using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class QuestionEditPage : ContentPage
{
    public QuestionEditPage(QuestionEditViewModel vm) { InitializeComponent(); BindingContext = vm; }
}