using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class ListadoReservasPage : ContentPage
{
    private readonly ReservaViewModel _viewModel;

    public ListadoReservasPage(ReservaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InicializarAsync();
    }
}