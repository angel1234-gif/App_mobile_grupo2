using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReservasBarberia.Models;
using ReservasBarberia.Services;

namespace ReservasBarberia.ViewModels;

public partial class ReservaViewModel : ObservableObject
{
    private readonly DataService _data;

    // Horario de atención de la barbería
    private static readonly TimeSpan Apertura = new(9, 0, 0);
    private static readonly TimeSpan Cierre = new(19, 0, 0);

    public ReservaViewModel(DataService data)
    {
        _data = data;
    }

    
    public ObservableCollection<Barbero> Barberos => _data.Barberos;
    public ObservableCollection<Servicio> Servicios => _data.Servicios;
    public ObservableCollection<Reserva> Reservas => _data.Reservas;

    // La fecha más temprana que se puede elegir en el calendario
    public DateTime FechaMinima => DateTime.Today;

    //Campos del formulario 
    [ObservableProperty] private string clienteNombre = string.Empty;
    [ObservableProperty] private string clienteTelefono = string.Empty;
    [ObservableProperty] private Barbero? barberoSeleccionado;
    [ObservableProperty] private Servicio? servicioSeleccionado;
    [ObservableProperty] private DateTime fecha = DateTime.Today;
    [ObservableProperty] private TimeSpan hora = new(9, 0, 0);

    //  Mensajes de error (uno por campo) 
    [ObservableProperty] private string errorNombre = string.Empty;
    [ObservableProperty] private string errorTelefono = string.Empty;
    [ObservableProperty] private string errorBarbero = string.Empty;
    [ObservableProperty] private string errorServicio = string.Empty;
    [ObservableProperty] private string errorFecha = string.Empty;
    [ObservableProperty] private string errorHora = string.Empty;

    // Cuando el usuario corrige un campo, su mensaje de error desaparece solo
    partial void OnClienteNombreChanged(string value) => ErrorNombre = string.Empty;
    partial void OnClienteTelefonoChanged(string value) => ErrorTelefono = string.Empty;
    partial void OnBarberoSeleccionadoChanged(Barbero? value)
    {
        ErrorBarbero = string.Empty;
        ErrorHora = string.Empty; // la disponibilidad de la hora depende del barbero
    }
    partial void OnServicioSeleccionadoChanged(Servicio? value) => ErrorServicio = string.Empty;
    partial void OnFechaChanged(DateTime value)
    {
        ErrorFecha = string.Empty;
        ErrorHora = string.Empty; // la validez de la hora depende de la fecha
    }
    partial void OnHoraChanged(TimeSpan value) => ErrorHora = string.Empty;

    
    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar())
            return;

        string digitos = SoloDigitos(ClienteTelefono);

        var reserva = new Reserva
        {
            ClienteNombre = ClienteNombre.Trim(),
            ClienteTelefono = $"{digitos[..3]}-{digitos[3..6]}-{digitos[6..]}",
            Barbero = BarberoSeleccionado!.Nombre,
            Servicio = ServicioSeleccionado!.Nombre,
            Fecha = Fecha.Date,
            Hora = Hora
        };

        _data.AgregarReserva(reserva);

        await Shell.Current.DisplayAlert(
            "¡Reserva guardada!",
            $"La cita de {reserva.ClienteNombre} con {reserva.Barbero} quedó registrada.",
            "OK");

        LimpiarFormulario();
    }

    // ---------- Validaciones ----------
    private bool Validar()
    {

        ErrorNombre = ErrorTelefono = ErrorBarbero = ErrorServicio = ErrorFecha = ErrorHora = string.Empty;

        // Nombre del cliente
        string nombre = ClienteNombre.Trim();
        if (nombre.Length == 0)
            ErrorNombre = "Escribe el nombre del cliente.";
        else if (nombre.Length < 3)
            ErrorNombre = "El nombre debe tener al menos 3 letras.";
        else if (nombre.Any(char.IsDigit))
            ErrorNombre = "El nombre no puede contener números.";

        // Teléfono (formato dominicano: 10 dígitos que empiezan con 809, 829 o 849)
        string digitos = SoloDigitos(ClienteTelefono);
        if (string.IsNullOrWhiteSpace(ClienteTelefono))
            ErrorTelefono = "Escribe el teléfono del cliente.";
        else if (ClienteTelefono.Any(char.IsLetter))
            ErrorTelefono = "El teléfono solo puede tener números.";
        else if (digitos.Length != 10)
            ErrorTelefono = "El teléfono debe tener 10 dígitos, por ejemplo 809-555-1234.";
        else if (!(digitos.StartsWith("809") || digitos.StartsWith("829") || digitos.StartsWith("849")))
            ErrorTelefono = "El teléfono debe empezar con 809, 829 o 849.";

        // Barbero y servicio
        var barbero = BarberoSeleccionado;
        if (barbero is null)
            ErrorBarbero = "Selecciona el barbero que atenderá la cita.";

        if (ServicioSeleccionado is null)
            ErrorServicio = "Selecciona el servicio que deseas.";

        // Fecha
        if (Fecha.Date < DateTime.Today)
            ErrorFecha = "La fecha no puede ser anterior a hoy.";

        // Hora
        if (Hora < Apertura || Hora > Cierre)
            ErrorHora = "La barbería atiende de 9:00 AM a 7:00 PM.";
        else if (Fecha.Date == DateTime.Today && Hora <= DateTime.Now.TimeOfDay)
            ErrorHora = "Esa hora ya pasó. Elige una hora posterior.";
        else if (barbero is not null && _data.Reservas.Any(r =>
                     r.Barbero == barbero.Nombre &&
                     r.Fecha.Date == Fecha.Date &&
                     r.Hora == Hora))
            ErrorHora = $"{barbero.Nombre} ya tiene una cita a esa hora. Elige otra.";

        // El formulario es válido solo si no quedó ningún mensaje de error
        return string.IsNullOrEmpty(ErrorNombre)
            && string.IsNullOrEmpty(ErrorTelefono)
            && string.IsNullOrEmpty(ErrorBarbero)
            && string.IsNullOrEmpty(ErrorServicio)
            && string.IsNullOrEmpty(ErrorFecha)
            && string.IsNullOrEmpty(ErrorHora);
    }

    private static string SoloDigitos(string texto)
    {
        return new string(texto.Where(char.IsDigit).ToArray());
    }

    private void LimpiarFormulario()
    {
        ClienteNombre = string.Empty;
        ClienteTelefono = string.Empty;
        BarberoSeleccionado = null;
        ServicioSeleccionado = null;
        Fecha = DateTime.Today;
        Hora = new TimeSpan(9, 0, 0);

        ErrorNombre = ErrorTelefono = ErrorBarbero = ErrorServicio = ErrorFecha = ErrorHora = string.Empty;
    }
}