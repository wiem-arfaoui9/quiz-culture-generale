using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class QuestionsPage : ContentPage
{
    private readonly QuestionsViewModel _vm;

    public QuestionsPage(QuestionsViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        SizeChanged += OnSizeChanged;
    }

    // Responsive : colonne centrée (760 px max) sur grand écran
    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (Width <= 0) return;
        double pad = Math.Max(16, (Width - 760) / 2);
        questionsList.Margin = new Thickness(pad, 0);
        heroBar.Padding = new Thickness(pad + 4, 8, pad + 4, 22);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
