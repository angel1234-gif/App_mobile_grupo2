using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class ListadoReservasPage : ContentPage
{
    public ListadoReservasPage(ReservaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}