using QuizApp.ViewModels;
namespace QuizApp.Views;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesViewModel _vm;

    public CategoriesPage(CategoriesViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        SizeChanged += OnSizeChanged;
    }

    // Responsive : contenu centré (1000 px max) et 1 à 3 tuiles par ligne selon la largeur
    private void OnSizeChanged(object sender, EventArgs e)
    {
        if (Width <= 0) return;

        double pad = Math.Max(16, (Width - 1000) / 2);
        categoriesList.Margin = new Thickness(pad, 0);
        heroBar.Padding = new Thickness(pad + 4, 40, pad + 4, 26);

        if (categoriesList.ItemsLayout is GridItemsLayout grid)
            grid.Span = Math.Clamp((int)((Width - 2 * pad + 14) / 314), 1, 3);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        categoriesList.Opacity = 0;
        categoriesList.TranslationY = 30;

        await _vm.LoadAsync();

        await Task.WhenAll(
            categoriesList.FadeToAsync(1, 450, Easing.CubicOut),
            categoriesList.TranslateToAsync(0, 0, 450, Easing.CubicOut));
    }
}
