using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class ServiciosPage : ContentPage
{
    public ServiciosPage(ServicioViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}