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

    
    public ObservableCollection<Cliente> Clientes => _data.Clientes;

   
    public event Action? IrAlFormulario;

    
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsEdicion))]
    [NotifyPropertyChangedFor(nameof(Titulo))]
    [NotifyPropertyChangedFor(nameof(TextoBotonGuardar))]
    private int? idEnEdicion;

    public bool EsEdicion => IdEnEdicion.HasValue;
    public string Titulo => EsEdicion ? "Editar cliente" : "Nuevo cliente";
    public string TextoBotonGuardar => EsEdicion ? "Guardar cambios" : "Guardar cliente";

    
    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string telefono = string.Empty;
    [ObservableProperty] private string correo = string.Empty;

    
    [ObservableProperty] private string errorNombre = string.Empty;
    [ObservableProperty] private string errorTelefono = string.Empty;
    [ObservableProperty] private string errorCorreo = string.Empty;

    
    partial void OnNombreChanged(string value) => ErrorNombre = string.Empty;
    partial void OnTelefonoChanged(string value) => ErrorTelefono = string.Empty;
    partial void OnCorreoChanged(string value) => ErrorCorreo = string.Empty;

    
    public void AlMostrarPantalla() => LimpiarFormulario();

    
    [RelayCommand]
    private void Editar(Cliente cliente)
    {
        if (cliente is null)
            return;

        IdEnEdicion = cliente.Id;
        Nombre = cliente.Nombre;
        Telefono = cliente.Telefono;
        Correo = cliente.Correo;

        LimpiarErrores();
        IrAlFormulario?.Invoke();
    }

    
    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar())
            return;

        var cliente = new Cliente
        {
            Id = IdEnEdicion ?? 0, 
            Nombre = Nombre.Trim(),
            Telefono = Validaciones.FormatearTelefono(Telefono),
            Correo = Correo.Trim().ToLower()
        };

        if (EsEdicion)
        {
            _data.ActualizarCliente(cliente);

            await Shell.Current.DisplayAlert(
                "¡Cambios guardados!",
                $"Los datos de {cliente.Nombre} se actualizaron.",
                "OK");
        }
        else
        {
            _data.AgregarCliente(cliente);

            await Shell.Current.DisplayAlert(
                "¡Cliente guardado!",
                $"{cliente.Nombre} quedó registrado.",
                "OK");
        }

        LimpiarFormulario();
    }

    
    [RelayCommand]
    private void CancelarEdicion() => LimpiarFormulario();

    
    [RelayCommand]
    private async Task EliminarDeListaAsync(Cliente cliente)
    {
        if (cliente is null)
            return;

        if (await ConfirmarYEliminarAsync(cliente) && IdEnEdicion == cliente.Id)
            LimpiarFormulario(); // si justo se estaba editando, se sale del modo edición
    }

    
    [RelayCommand]
    private async Task EliminarActualAsync()
    {
        var cliente = _data.Clientes.FirstOrDefault(c => c.Id == IdEnEdicion);
        if (cliente is null)
            return;

        if (await ConfirmarYEliminarAsync(cliente))
            LimpiarFormulario();
    }

    
    private async Task<bool> ConfirmarYEliminarAsync(Cliente cliente)
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar cliente",
            $"¿Seguro que quieres eliminar a {cliente.Nombre}? Esta acción no se puede deshacer.",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar)
            return false;

        _data.EliminarCliente(cliente);
        return true;
    }

    
    private bool Validar()
    {
        LimpiarErrores();

        
        ErrorNombre = Validaciones.Nombre(Nombre, "del cliente");

        
        ErrorTelefono = Validaciones.Telefono(Telefono, "del cliente");
        if (string.IsNullOrEmpty(ErrorTelefono))
        {
            string digitos = Validaciones.SoloDigitos(Telefono);
            if (_data.Clientes.Any(c => c.Id != IdEnEdicion &&
                                        Validaciones.SoloDigitos(c.Telefono) == digitos))
                ErrorTelefono = "Ya hay un cliente registrado con ese teléfono.";
        }

        
        ErrorCorreo = ValidarCorreo(Correo);
        if (string.IsNullOrEmpty(ErrorCorreo) &&
            _data.Clientes.Any(c => c.Id != IdEnEdicion &&
                                    c.Correo.Equals(Correo.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ErrorCorreo = "Ya hay un cliente registrado con ese correo.";
        }

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

        
        bool formatoValido = MailAddress.TryCreate(correo, out var direccion)
                             && direccion.Address == correo
                             && direccion.Host.Contains('.');

        if (!formatoValido)
            return "Escribe un correo válido, por ejemplo: nombre@correo.com";

        return string.Empty;
    }

    private void LimpiarErrores()
    {
        ErrorNombre = ErrorTelefono = ErrorCorreo = string.Empty;
    }

    
    private void LimpiarFormulario()
    {
        IdEnEdicion = null;
        Nombre = string.Empty;
        Telefono = string.Empty;
        Correo = string.Empty;

        LimpiarErrores();
    }
}

