using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class ServiciosPage : ContentPage
{
    private readonly ServicioViewModel _viewModel;

    public ServiciosPage(ServicioViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        
        _viewModel.IrAlFormulario += async () => await Desplazador.ScrollToAsync(0, 0, true);
    }

    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.AlMostrarPantalla();
    }
}