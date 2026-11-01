using System.Collections.ObjectModel;
using System.Net.Mail;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReservasBarberia.Models;
using ReservasBarberia.Services;
using ReservasBarberia.Utilidades;

namespace ReservasBarberia.ViewModels;

public partial class ClienteViewModel : ObservableObject
{
    private readonly DataService _data;

    public ClienteViewModel(DataService data)
    {
        _data = data;
    }

    // Lista que muestra el listado (viene del servicio en memoria)
    public ObservableCollection<Cliente> Clientes => _data.Clientes;

    //  Campos del formulario 
    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string telefono = string.Empty;
    [ObservableProperty] private string correo = string.Empty;

    // Mensajes de error (uno por campo) 
    [ObservableProperty] private string errorNombre = string.Empty;
    [ObservableProperty] private string errorTelefono = string.Empty;
    [ObservableProperty] private string errorCorreo = string.Empty;

    // Al corregir un campo, su mensaje de error desaparece solo
    partial void OnNombreChanged(string value) => ErrorNombre = string.Empty;
    partial void OnTelefonoChanged(string value) => ErrorTelefono = string.Empty;
    partial void OnCorreoChanged(string value) => ErrorCorreo = string.Empty;

    //  Botón "Guardar cliente" 
    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar())
            return;

        var cliente = new Cliente
        {
            Nombre = Nombre.Trim(),
            Telefono = Validaciones.FormatearTelefono(Telefono),
            Correo = Correo.Trim().ToLower()
        };

        _data.AgregarCliente(cliente);

        await Shell.Current.DisplayAlert(
            "¡Cliente guardado!",
            $"{cliente.Nombre} quedó registrado.",
            "OK");

        LimpiarFormulario();
    }

    // Validaciones 
    private bool Validar()
    {
        // Nombre: obligatorio mínimo 3 letras y sin números
        ErrorNombre = Validaciones.Nombre(Nombre, "del cliente");

        // Teléfono dominicano, y que no esté ya registrado por otro cliente
        ErrorTelefono = Validaciones.Telefono(Telefono, "del cliente");
        if (string.IsNullOrEmpty(ErrorTelefono))
        {
            string digitos = Validaciones.SoloDigitos(Telefono);
            if (_data.Clientes.Any(c => Validaciones.SoloDigitos(c.Telefono) == digitos))
                ErrorTelefono = "Ya hay un cliente registrado con ese teléfono.";
        }

        // Correo: obligatorio, con formato válido y sin repetirse
        ErrorCorreo = ValidarCorreo(Correo);
        if (string.IsNullOrEmpty(ErrorCorreo) &&
            _data.Clientes.Any(c => c.Correo.Equals(Correo.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ErrorCorreo = "Ya hay un cliente registrado con ese correo.";
        }

        // Válido solo si ningún campo quedó con error
        return string.IsNullOrEmpty(ErrorNombre)
            && string.IsNullOrEmpty(ErrorTelefono)
            && string.IsNullOrEmpty(ErrorCorreo);
    }

    private static string ValidarCorreo(string valor)
    {
        string correo = valor.Trim();

        if (correo.Length == 0)
            return "Escribe el correo del cliente.";

        if (correo.Contains(' '))
            return "El correo no puede tener espacios.";

        // Debe tener forma de correo: juan@gmail.ext
        bool formatoValido = MailAddress.TryCreate(correo, out var direccion)
                             && direccion.Address == correo
                             && direccion.Host.Contains('.');

        if (!formatoValido)
            return "Escribe un correo válido, por ejemplo: nombre@correo.com";

        return string.Empty;
    }

    private void LimpiarFormulario()
    {
        Nombre = string.Empty;
        Telefono = string.Empty;
        Correo = string.Empty;

        ErrorNombre = ErrorTelefono = ErrorCorreo = string.Empty;
    }
}