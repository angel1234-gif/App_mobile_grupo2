using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class ClientesPage : ContentPage
{
    public ClientesPage(ClienteViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}