using QuizApp.Views;
namespace QuizApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("register", typeof(RegisterPage));
        Routing.RegisterRoute("questions", typeof(QuestionsPage));
        Routing.RegisterRoute("questionedit", typeof(QuestionEditPage));
        Routing.RegisterRoute("quiz", typeof(QuizPage));
        Routing.RegisterRoute("result", typeof(ResultPage));
    }
}