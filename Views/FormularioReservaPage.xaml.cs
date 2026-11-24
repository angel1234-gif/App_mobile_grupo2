using ReservasBarberia.ViewModels;

namespace ReservasBarberia.Views;

public partial class FormularioReservaPage : ContentPage
{
    private readonly ReservaViewModel _viewModel;

    public FormularioReservaPage(ReservaViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.AlMostrarFormulario();
    }
}