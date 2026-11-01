using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ReservasBarberia.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    
    private const string UsuarioValido = "admin";
    private const string ClaveValida = "1234";

    //  Campos del formulario
    [ObservableProperty] private string usuario = string.Empty;
    [ObservableProperty] private string clave = string.Empty;

    
    [ObservableProperty] private string errorUsuario = string.Empty;
    [ObservableProperty] private string errorClave = string.Empty;

    // Al corregir un campo, su mensaje de error desaparece solo
    partial void OnUsuarioChanged(string value) => ErrorUsuario = string.Empty;
    partial void OnClaveChanged(string value) => ErrorClave = string.Empty;

    // ---------- Botón "Iniciar sesión" ----------
    [RelayCommand]
    private async Task IngresarAsync()
    {
        if (!Validar())
            return;

        
        Usuario = string.Empty;
        Clave = string.Empty;

        await Shell.Current.GoToAsync("//PrincipalPage");
    }

    // ---------- Validaciones ----------
    private bool Validar()
    {
        ErrorUsuario = string.Empty;
        ErrorClave = string.Empty;

        string usuarioLimpio = Usuario.Trim();

        // Usuario
        if (usuarioLimpio.Length == 0)
            ErrorUsuario = "Escribe tu usuario.";
        else if (usuarioLimpio.Contains(' '))
            ErrorUsuario = "El usuario no puede tener espacios.";

        // Contraseña
        if (Clave.Length == 0)
            ErrorClave = "Escribe tu contraseña.";
        else if (Clave.Length < 4)
            ErrorClave = "La contraseña debe tener al menos 4 caracteres.";

        // Si el formato de los dos campos está bien, se comparan con las credenciales fijas
        if (string.IsNullOrEmpty(ErrorUsuario) && string.IsNullOrEmpty(ErrorClave))
        {
            bool correctas = usuarioLimpio.Equals(UsuarioValido, StringComparison.OrdinalIgnoreCase)
                             && Clave == ClaveValida;

            if (!correctas)
                ErrorClave = "Usuario o contraseña incorrectos.";
        }

        return string.IsNullOrEmpty(ErrorUsuario) && string.IsNullOrEmpty(ErrorClave);
    }
}