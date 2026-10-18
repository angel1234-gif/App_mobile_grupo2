namespace ReservasBarberia.Views;

public partial class LoginPage : ContentPage
{
    
    private const string UsuarioValido = "admin";
    private const string ClaveValida = "1234";

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string usuario = UsuarioEntry.Text?.Trim() ?? string.Empty;
        string clave = ClaveEntry.Text ?? string.Empty;

        
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
        {
            MostrarError("Escribe tu usuario y tu contraseña.");
            return;
        }

        
        if (usuario == UsuarioValido && clave == ClaveValida)
        {
            MensajeError.IsVisible = false;
            UsuarioEntry.Text = string.Empty;
            ClaveEntry.Text = string.Empty;

            await Shell.Current.GoToAsync("//PrincipalPage");
        }
        
        else
        {
            MostrarError("Usuario o contraseña incorrectos.");
        }
    }

    private void MostrarError(string mensaje)
    {
        MensajeError.Text = mensaje;
        MensajeError.IsVisible = true;
    }
}