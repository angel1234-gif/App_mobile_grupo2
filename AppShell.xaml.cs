namespace ReservasBarberia;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        bool confirmar = await DisplayAlert(
            "Cerrar sesión",
            "¿Seguro que quieres cerrar sesión?",
            "Sí",
            "No");

        if (!confirmar)
            return;

        Shell.Current.FlyoutIsPresented = false;
        await Shell.Current.GoToAsync("//LoginPage");
    }
}