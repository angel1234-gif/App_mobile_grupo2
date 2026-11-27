using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class BarberosPage : ContentPage
{
    private readonly BarberoViewModel _viewModel;

    public BarberosPage(BarberoViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        _viewModel.IrAlFormulario += async () => await Desplazador.ScrollToAsync(0, 0, true);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.AlMostrarPantallaAsync();
    }
}