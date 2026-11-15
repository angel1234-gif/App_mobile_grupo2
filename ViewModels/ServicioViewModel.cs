using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReservasBarberia.Models;
using ReservasBarberia.Services;

namespace ReservasBarberia.ViewModels;

public partial class ServicioViewModel : ObservableObject
{
    
    private const decimal PrecioMaximo = 10000;
    private const int DuracionMinima = 10;
    private const int DuracionMaxima = 180;

    private readonly DataService _data;

    public ServicioViewModel(DataService data)
    {
        _data = data;
    }

    
    public ObservableCollection<Servicio> Servicios => _data.Servicios;

 
    public event Action? IrAlFormulario;

   
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsEdicion))]
    [NotifyPropertyChangedFor(nameof(Titulo))]
    [NotifyPropertyChangedFor(nameof(TextoBotonGuardar))]
    private int? idEnEdicion;

    public bool EsEdicion => IdEnEdicion.HasValue;
    public string Titulo => EsEdicion ? "Editar servicio" : "Nuevo servicio";
    public string TextoBotonGuardar => EsEdicion ? "Guardar cambios" : "Guardar servicio";

    
    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string precio = string.Empty;
    [ObservableProperty] private string duracion = string.Empty;

    
    [ObservableProperty] private string errorNombre = string.Empty;
    [ObservableProperty] private string errorPrecio = string.Empty;
    [ObservableProperty] private string errorDuracion = string.Empty;

    
    partial void OnNombreChanged(string value) => ErrorNombre = string.Empty;
    partial void OnPrecioChanged(string value) => ErrorPrecio = string.Empty;
    partial void OnDuracionChanged(string value) => ErrorDuracion = string.Empty;

    
    public void AlMostrarPantalla() => LimpiarFormulario();

    
    [RelayCommand]
    private void Editar(Servicio servicio)
    {
        if (servicio is null)
            return;

        IdEnEdicion = servicio.Id;
        Nombre = servicio.Nombre;
        Precio = servicio.Precio.ToString("0.##", CultureInfo.CurrentCulture);
        Duracion = servicio.DuracionMinutos.ToString();

        LimpiarErrores();
        IrAlFormulario?.Invoke();
    }

    
    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar(out decimal precioValor, out int duracionValor))
            return;

        var servicio = new Servicio
        {
            Id = IdEnEdicion ?? 0, 
            Nombre = Nombre.Trim(),
            Precio = precioValor,
            DuracionMinutos = duracionValor
        };

        if (EsEdicion)
        {
            _data.ActualizarServicio(servicio);

            await Shell.Current.DisplayAlert(
                "¡Cambios guardados!",
                $"El servicio {servicio.Nombre} se actualizó.",
                "OK");
        }
        else
        {
            _data.AgregarServicio(servicio);

            await Shell.Current.DisplayAlert(
                "¡Servicio guardado!",
                $"{servicio.Nombre}: RD${servicio.Precio:N2} · {servicio.DuracionMinutos} min.",
                "OK");
        }

        LimpiarFormulario();
    }

    
    [RelayCommand]
    private void CancelarEdicion() => LimpiarFormulario();

    
    [RelayCommand]
    private async Task EliminarDeListaAsync(Servicio servicio)
    {
        if (servicio is null)
            return;

        if (await ConfirmarYEliminarAsync(servicio) && IdEnEdicion == servicio.Id)
            LimpiarFormulario(); 
    }

    
    [RelayCommand]
    private async Task EliminarActualAsync()
    {
        var servicio = _data.Servicios.FirstOrDefault(s => s.Id == IdEnEdicion);
        if (servicio is null)
            return;

        if (await ConfirmarYEliminarAsync(servicio))
            LimpiarFormulario();
    }

    
    private async Task<bool> ConfirmarYEliminarAsync(Servicio servicio)
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar servicio",
            $"¿Seguro que quieres eliminar el servicio {servicio.Nombre}? Esta acción no se puede deshacer.",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar)
            return false;

        _data.EliminarServicio(servicio);
        return true;
    }


    private bool Validar(out decimal precioValor, out int duracionValor)
    {
        LimpiarErrores();
        precioValor = 0;
        duracionValor = 0;

        
        string nombreLimpio = Nombre.Trim();
        if (nombreLimpio.Length == 0)
            ErrorNombre = "Escribe el nombre del servicio.";
        else if (nombreLimpio.Length < 3)
            ErrorNombre = "El nombre debe tener al menos 3 letras.";
        else if (_data.Servicios.Any(s => s.Id != IdEnEdicion &&
                                          s.Nombre.Equals(nombreLimpio, StringComparison.OrdinalIgnoreCase)))
            ErrorNombre = "Ya existe un servicio con ese nombre.";

        
        string precioTexto = Precio.Trim();
        if (precioTexto.Length == 0)
            ErrorPrecio = "Escribe el precio del servicio.";
        else if (!decimal.TryParse(precioTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out precioValor))
            ErrorPrecio = "El precio debe ser un número, por ejemplo 350.";
        else if (precioValor <= 0)
            ErrorPrecio = "El precio debe ser mayor que 0.";
        else if (precioValor > PrecioMaximo)
            ErrorPrecio = $"El precio no puede superar RD${PrecioMaximo:N0}.";

        
        string duracionTexto = Duracion.Trim();
        if (duracionTexto.Length == 0)
            ErrorDuracion = "Escribe la duración en minutos.";
        else if (!int.TryParse(duracionTexto, NumberStyles.Integer, CultureInfo.InvariantCulture, out duracionValor))
            ErrorDuracion = "La duración debe ser un número entero de minutos, por ejemplo 30.";
        else if (duracionValor < DuracionMinima || duracionValor > DuracionMaxima)
            ErrorDuracion = $"La duración debe estar entre {DuracionMinima} y {DuracionMaxima} minutos.";

        return string.IsNullOrEmpty(ErrorNombre)
            && string.IsNullOrEmpty(ErrorPrecio)
            && string.IsNullOrEmpty(ErrorDuracion);
    }

    private void LimpiarErrores()
    {
        ErrorNombre = ErrorPrecio = ErrorDuracion = string.Empty;
    }

    


    private void LimpiarFormulario()
    {
        IdEnEdicion = null;
        Nombre = string.Empty;
        Precio = string.Empty;
        Duracion = string.Empty;

        LimpiarErrores();
    }
}