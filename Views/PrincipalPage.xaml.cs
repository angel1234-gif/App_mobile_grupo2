namespace ReservasBarberia.Views;

public partial class PrincipalPage : ContentPage
{
    public PrincipalPage()
    {
        InitializeComponent();
    }

    private async void OnVerReservasClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ListadoReservasPage");
    }

    private async void OnNuevaReservaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//FormularioReservaPage");
    }
}