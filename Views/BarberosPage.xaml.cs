using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class BarberosPage : ContentPage
{
    public BarberosPage(BarberoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}