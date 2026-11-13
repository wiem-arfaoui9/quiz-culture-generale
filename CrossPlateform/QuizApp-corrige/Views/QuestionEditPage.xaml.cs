using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class QuestionEditPage : ContentPage
{
    public QuestionEditPage(QuestionEditViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;

        // Responsive : formulaire centré (680 px max) sur grand écran
        SizeChanged += (_, _) =>
        {
            if (Width > 0) content.Padding = new Thickness(Math.Max(16, (Width - 680) / 2), 16);
        };
    }
}
