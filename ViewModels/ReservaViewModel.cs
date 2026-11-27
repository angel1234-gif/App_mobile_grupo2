using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using ReservasBarberia.Models;
using ReservasBarberia.Services;

namespace ReservasBarberia.ViewModels;

public partial class ReservaViewModel : ObservableObject
{
    private readonly DataService _data;

    private static readonly TimeSpan Apertura = new(9, 0, 0);
    private static readonly TimeSpan Cierre = new(19, 0, 0);

    public ReservaViewModel(DataService data)
    {
        _data = data;
        _data.Reservas.CollectionChanged += (_, _) => AplicarFiltros();
        _data.Barberos.CollectionChanged += (_, _) => ActualizarOpcionesBarbero();
    }

    public ObservableCollection<Barbero> Barberos => _data.Barberos;
    public ObservableCollection<Servicio> Servicios => _data.Servicios;
    public ObservableCollection<Reserva> Reservas => _data.Reservas;

    public DateTime FechaMinima => DateTime.Today;

    // ===== Innovación 1: búsqueda y filtros del listado =====
    private const string TodosLosBarberos = "Todos los barberos";

    public ObservableCollection<Reserva> ReservasFiltradas { get; } = new();
    public ObservableCollection<string> OpcionesBarbero { get; } = new();
    public List<string> OpcionesPeriodo { get; } = new() { "Todas", "Hoy", "Próximas", "Pasadas" };

    [ObservableProperty] private string textoBusqueda = string.Empty;
    [ObservableProperty] private string? filtroBarbero = TodosLosBarberos;
    [ObservableProperty] private string filtroPeriodo = "Todas";
    [ObservableProperty] private string resumen = string.Empty;

    partial void OnTextoBusquedaChanged(string value) => AplicarFiltros();
    partial void OnFiltroBarberoChanged(string? value) => AplicarFiltros();
    partial void OnFiltroPeriodoChanged(string value) => AplicarFiltros();

    private void AplicarFiltros()
    {
        string texto = TextoBusqueda.Trim();
        string digitosBuscados = SoloDigitos(texto);
        string barbero = FiltroBarbero ?? TodosLosBarberos;
        DateTime hoy = DateTime.Today;

        var resultado = _data.Reservas.Where(r =>
        {
            bool coincideTexto =
                texto.Length == 0
                || r.ClienteNombre.Contains(texto, StringComparison.OrdinalIgnoreCase)
                || (digitosBuscados.Length > 0
                    && SoloDigitos(r.ClienteTelefono).Contains(digitosBuscados));

            bool coincideBarbero =
                barbero == TodosLosBarberos || r.Barbero == barbero;

            bool coincidePeriodo = FiltroPeriodo switch
            {
                "Hoy" => r.Fecha.Date == hoy,
                "Próximas" => r.Fecha.Date > hoy,
                "Pasadas" => r.Fecha.Date < hoy,
                _ => true
            };

            return coincideTexto && coincideBarbero && coincidePeriodo;
        }).ToList();

        ReservasFiltradas.Clear();
        foreach (var reserva in resultado)
            ReservasFiltradas.Add(reserva);

        Resumen = $"{resultado.Count} de {_data.Reservas.Count} reservas";
    }

    private void ActualizarOpcionesBarbero()
    {
        string? seleccionado = FiltroBarbero;

        OpcionesBarbero.Clear();
        OpcionesBarbero.Add(TodosLosBarberos);
        foreach (var b in _data.Barberos)
            OpcionesBarbero.Add(b.Nombre);

        FiltroBarbero = seleccionado is not null && OpcionesBarbero.Contains(seleccionado)
            ? seleccionado
            : TodosLosBarberos;
    }

    [RelayCommand]
    private void LimpiarFiltros()
    {
        TextoBusqueda = string.Empty;
        FiltroBarbero = TodosLosBarberos;
        FiltroPeriodo = "Todas";
    }

    // ===== Formulario de reserva =====
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsEdicion))]
    [NotifyPropertyChangedFor(nameof(Titulo))]
    [NotifyPropertyChangedFor(nameof(TextoBotonGuardar))]
    private int? idEnEdicion;

    public bool EsEdicion => IdEnEdicion.HasValue;
    public string Titulo => EsEdicion ? "Editar reserva" : "Nueva reserva";
    public string TextoBotonGuardar => EsEdicion ? "Guardar cambios" : "Guardar reserva";

    [ObservableProperty] private string clienteNombre = string.Empty;
    [ObservableProperty] private string clienteTelefono = string.Empty;
    [ObservableProperty] private Barbero? barberoSeleccionado;
    [ObservableProperty] private Servicio? servicioSeleccionado;
    [ObservableProperty] private DateTime fecha = DateTime.Today;
    [ObservableProperty] private TimeSpan hora = new(9, 0, 0);

    [ObservableProperty] private string errorNombre = string.Empty;
    [ObservableProperty] private string errorTelefono = string.Empty;
    [ObservableProperty] private string errorBarbero = string.Empty;
    [ObservableProperty] private string errorServicio = string.Empty;
    [ObservableProperty] private string errorFecha = string.Empty;
    [ObservableProperty] private string errorHora = string.Empty;

    partial void OnClienteNombreChanged(string value) => ErrorNombre = string.Empty;
    partial void OnClienteTelefonoChanged(string value) => ErrorTelefono = string.Empty;
    partial void OnBarberoSeleccionadoChanged(Barbero? value)
    {
        ErrorBarbero = string.Empty;
        ErrorHora = string.Empty;
    }
    partial void OnServicioSeleccionadoChanged(Servicio? value) => ErrorServicio = string.Empty;
    partial void OnFechaChanged(DateTime value)
    {
        ErrorFecha = string.Empty;
        ErrorHora = string.Empty;
    }
    partial void OnHoraChanged(TimeSpan value) => ErrorHora = string.Empty;

    // Inicialización lazy de la BD (se llama desde OnAppearing del listado)
    public async Task InicializarAsync()
    {
        await _data.InicializarAsync();
        ActualizarOpcionesBarbero();
        AplicarFiltros();
    }

    // Se llama desde OnAppearing del formulario
    public async Task AlMostrarFormularioAsync()
    {
        await _data.InicializarAsync();

        var reserva = _data.ReservaPorEditar;
        _data.ReservaPorEditar = null; // se usa una sola vez

        if (reserva is null)
            LimpiarFormulario();
        else
            CargarParaEditar(reserva);
    }

    private void CargarParaEditar(Reserva reserva)
    {
        IdEnEdicion = reserva.Id;
        ClienteNombre = reserva.ClienteNombre;
        ClienteTelefono = reserva.ClienteTelefono;
        BarberoSeleccionado = Barberos.FirstOrDefault(b => b.Nombre == reserva.Barbero);
        ServicioSeleccionado = Servicios.FirstOrDefault(s => s.Nombre == reserva.Servicio);
        Fecha = reserva.Fecha;
        Hora = reserva.Hora;

        LimpiarErrores();
    }

    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar())
            return;

        string digitos = SoloDigitos(ClienteTelefono);

        var reserva = new Reserva
        {
            Id = IdEnEdicion ?? 0,
            ClienteNombre = ClienteNombre.Trim(),
            ClienteTelefono = $"{digitos[..3]}-{digitos[3..6]}-{digitos[6..]}",
            Barbero = BarberoSeleccionado!.Nombre,
            Servicio = ServicioSeleccionado!.Nombre,
            Fecha = Fecha.Date,
            Hora = Hora
        };

        try
        {
            if (EsEdicion)
            {
                await _data.ActualizarReservaAsync(reserva);

                await Shell.Current.DisplayAlert(
                    "¡Cambios guardados!",
                    $"La reserva de {reserva.ClienteNombre} se actualizó.",
                    "OK");

                LimpiarFormulario();
                await Shell.Current.GoToAsync("//ListadoReservasPage"); // vuelve a la lista para ver el cambio
            }
            else
            {
                await _data.AgregarReservaAsync(reserva);

                await Shell.Current.DisplayAlert(
                    "¡Reserva guardada!",
                    $"La cita de {reserva.ClienteNombre} con {reserva.Barbero} quedó registrada.",
                    "OK");

                LimpiarFormulario();
            }
        }
        catch (RegistroDuplicadoException)
        {
            
            ErrorHora = $"{reserva.Barbero} ya tiene una cita a esa hora. Elige otra.";
        }
    }

    [RelayCommand]
    private async Task CancelarEdicionAsync()
    {
        LimpiarFormulario();
        await Shell.Current.GoToAsync("//ListadoReservasPage");
    }

    [RelayCommand]
    private async Task EditarAsync(Reserva reserva)
    {
        if (reserva is null)
            return;

        _data.ReservaPorEditar = reserva;
        await Shell.Current.GoToAsync("//FormularioReservaPage");
    }

    
    [RelayCommand]
    private async Task ConfirmarPorWhatsAppAsync(Reserva reserva)
    {
        if (reserva is null)
            return;

        
        string numero = "1" + SoloDigitos(reserva.ClienteTelefono);

        string hora = DateTime.Today.Add(reserva.Hora).ToString("hh:mm tt");

        string mensaje =
            $"Hola {reserva.ClienteNombre}, te confirmamos tu cita en la barbería:\n" +
            $"Servicio: {reserva.Servicio}\n" +
            $"Barbero: {reserva.Barbero}\n" +
            $"Fecha: {reserva.Fecha:dd/MM/yyyy}\n" +
            $"Hora: {hora}\n" +
            "¡Te esperamos!";

        string url = $"https://wa.me/{numero}?text={Uri.EscapeDataString(mensaje)}";

        try
        {
            await Launcher.Default.OpenAsync(new Uri(url));
        }
        catch
        {
            
            await Clipboard.Default.SetTextAsync(mensaje);

            await Shell.Current.DisplayAlert(
                "No se pudo abrir WhatsApp",
                "El mensaje se copió al portapapeles para que lo pegues en el chat.",
                "OK");

            return;
        }

        
        var confirmada = new Reserva
        {
            Id = reserva.Id,
            ClienteNombre = reserva.ClienteNombre,
            ClienteTelefono = reserva.ClienteTelefono,
            Barbero = reserva.Barbero,
            Servicio = reserva.Servicio,
            Fecha = reserva.Fecha,
            Hora = reserva.Hora,
            Confirmada = true
        };

        await _data.ActualizarReservaAsync(confirmada);
    }

    [RelayCommand]
    private async Task EliminarDeListaAsync(Reserva reserva)
    {
        if (reserva is null)
            return;

        await ConfirmarYEliminarAsync(reserva);
    }

    [RelayCommand]
    private async Task EliminarActualAsync()
    {
        var reserva = _data.Reservas.FirstOrDefault(r => r.Id == IdEnEdicion);
        if (reserva is null)
            return;

        if (await ConfirmarYEliminarAsync(reserva))
        {
            LimpiarFormulario();
            await Shell.Current.GoToAsync("//ListadoReservasPage");
        }
    }

    private async Task<bool> ConfirmarYEliminarAsync(Reserva reserva)
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar reserva",
            $"¿Seguro que quieres eliminar la reserva de {reserva.ClienteNombre} ({reserva.FechaHoraTexto})? Esta acción no se puede deshacer.",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar)
            return false;

        await _data.EliminarReservaAsync(reserva);
        return true;
    }

    private bool Validar()
    {
        LimpiarErrores();

        string nombre = ClienteNombre.Trim();
        if (nombre.Length == 0)
            ErrorNombre = "Escribe el nombre del cliente.";
        else if (nombre.Length < 3)
            ErrorNombre = "El nombre debe tener al menos 3 letras.";
        else if (nombre.Any(char.IsDigit))
            ErrorNombre = "El nombre no puede contener números.";

        string digitos = SoloDigitos(ClienteTelefono);
        if (string.IsNullOrWhiteSpace(ClienteTelefono))
            ErrorTelefono = "Escribe el teléfono del cliente.";
        else if (ClienteTelefono.Any(char.IsLetter))
            ErrorTelefono = "El teléfono solo puede tener números.";
        else if (digitos.Length != 10)
            ErrorTelefono = "El teléfono debe tener 10 dígitos, por ejemplo 809-555-1234.";
        else if (!(digitos.StartsWith("809") || digitos.StartsWith("829") || digitos.StartsWith("849")))
            ErrorTelefono = "El teléfono debe empezar con 809, 829 o 849.";

        var barbero = BarberoSeleccionado;
        if (barbero is null)
            ErrorBarbero = "Selecciona el barbero que atenderá la cita.";

        if (ServicioSeleccionado is null)
            ErrorServicio = "Selecciona el servicio que deseas.";

        if (Fecha.Date < DateTime.Today)
            ErrorFecha = "La fecha no puede ser anterior a hoy.";

        if (Hora < Apertura || Hora > Cierre)
            ErrorHora = "La barbería atiende de 9:00 AM a 7:00 PM.";
        else if (Fecha.Date == DateTime.Today && Hora <= DateTime.Now.TimeOfDay)
            ErrorHora = "Esa hora ya pasó. Elige una hora posterior.";
        else if (barbero is not null && _data.Reservas.Any(r =>
                     r.Id != IdEnEdicion &&
                     r.Barbero == barbero.Nombre &&
                     r.Fecha.Date == Fecha.Date &&
                     r.Hora == Hora))
            ErrorHora = $"{barbero.Nombre} ya tiene una cita a esa hora. Elige otra.";

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

    private void LimpiarErrores()
    {
        ErrorNombre = ErrorTelefono = ErrorBarbero = ErrorServicio = ErrorFecha = ErrorHora = string.Empty;
    }

    private void LimpiarFormulario()
    {
        IdEnEdicion = null;
        ClienteNombre = string.Empty;
        ClienteTelefono = string.Empty;
        BarberoSeleccionado = null;
        ServicioSeleccionado = null;
        Fecha = DateTime.Today;
        Hora = new TimeSpan(9, 0, 0);

        LimpiarErrores();
    }
}