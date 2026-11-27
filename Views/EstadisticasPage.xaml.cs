using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class EstadisticasPage : ContentPage
{
    private readonly EstadisticasViewModel _viewModel;

    public EstadisticasPage(EstadisticasViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarAsync();
    }
}