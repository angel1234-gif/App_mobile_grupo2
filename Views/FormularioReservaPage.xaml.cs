using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class FormularioReservaPage : ContentPage
{
    public FormularioReservaPage(ReservaViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}