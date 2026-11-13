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

    // Contenu centré (1000 px max) et jusqu'à 3 tuiles par ligne selon la largeur
    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (Width <= 0) return;

        double pad = Math.Max(16, (Width - 1000) / 2);
        categoriesList.Margin = new Thickness(pad, 0);
        heroBar.Padding = new Thickness(pad + 4, 6, pad + 4, 24);

        if (categoriesList.ItemsLayout is GridItemsLayout grid)
            grid.Span = Math.Clamp((int)((Width - 2 * pad + 12) / 312), 1, 3);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}