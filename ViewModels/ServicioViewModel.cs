using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReservasBarberia.Models;
using ReservasBarberia.Services;

namespace ReservasBarberia.ViewModels;

public partial class ServicioViewModel : ObservableObject
{
    // Límites que usan las validaciones
    private const decimal PrecioMaximo = 10000;
    private const int DuracionMinima = 10;
    private const int DuracionMaxima = 180;

    private readonly DataService _data;

    public ServicioViewModel(DataService data)
    {
        _data = data;
    }

    // Lista que muestra el listado (viene del servicio en memoria)
    public ObservableCollection<Servicio> Servicios => _data.Servicios;

    // ---------- Campos del formulario (se escriben como texto) ----------
    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string precio = string.Empty;
    [ObservableProperty] private string duracion = string.Empty;

    // ---------- Mensajes de error (uno por campo) ----------
    [ObservableProperty] private string errorNombre = string.Empty;
    [ObservableProperty] private string errorPrecio = string.Empty;
    [ObservableProperty] private string errorDuracion = string.Empty;

    // Al corregir un campo, su mensaje de error desaparece solo
    partial void OnNombreChanged(string value) => ErrorNombre = string.Empty;
    partial void OnPrecioChanged(string value) => ErrorPrecio = string.Empty;
    partial void OnDuracionChanged(string value) => ErrorDuracion = string.Empty;

    // ---------- Botón "Guardar servicio" ----------
    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar(out decimal precioValor, out int duracionValor))
            return;

        var servicio = new Servicio
        {
            Nombre = Nombre.Trim(),
            Precio = precioValor,
            DuracionMinutos = duracionValor
        };

        _data.AgregarServicio(servicio);

        await Shell.Current.DisplayAlert(
            "¡Servicio guardado!",
            $"{servicio.Nombre}: RD${servicio.Precio:N2} · {servicio.DuracionMinutos} min.",
            "OK");

        LimpiarFormulario();
    }

    // ---------- Validaciones ----------
    // Devuelve el precio y la duración ya convertidos a número cuando son válidos
    private bool Validar(out decimal precioValor, out int duracionValor)
    {
        precioValor = 0;
        duracionValor = 0;

        // Nombre: obligatorio, mínimo 3 caracteres y sin repetirse
        string nombreLimpio = Nombre.Trim();
        if (nombreLimpio.Length == 0)
            ErrorNombre = "Escribe el nombre del servicio.";
        else if (nombreLimpio.Length < 3)
            ErrorNombre = "El nombre debe tener al menos 3 letras.";
        else if (_data.Servicios.Any(s => s.Nombre.Equals(nombreLimpio, StringComparison.OrdinalIgnoreCase)))
            ErrorNombre = "Ya existe un servicio con ese nombre.";
        else
            ErrorNombre = string.Empty;

        // Precio: debe ser un número, mayor que 0 y no excesivo
        string precioTexto = Precio.Trim();
        if (precioTexto.Length == 0)
            ErrorPrecio = "Escribe el precio del servicio.";
        else if (!decimal.TryParse(precioTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out precioValor))
            ErrorPrecio = "El precio debe ser un número, por ejemplo 350.";
        else if (precioValor <= 0)
            ErrorPrecio = "El precio debe ser mayor que 0.";
        else if (precioValor > PrecioMaximo)
            ErrorPrecio = $"El precio no puede superar RD${PrecioMaximo:N0}.";
        else
            ErrorPrecio = string.Empty;

        // Duración: número entero de minutos dentro de un rango razonable
        string duracionTexto = Duracion.Trim();
        if (duracionTexto.Length == 0)
            ErrorDuracion = "Escribe la duración en minutos.";
        else if (!int.TryParse(duracionTexto, NumberStyles.Integer, CultureInfo.InvariantCulture, out duracionValor))
            ErrorDuracion = "La duración debe ser un número entero de minutos, por ejemplo 30.";
        else if (duracionValor < DuracionMinima || duracionValor > DuracionMaxima)
            ErrorDuracion = $"La duración debe estar entre {DuracionMinima} y {DuracionMaxima} minutos.";
        else
            ErrorDuracion = string.Empty;

        // Válido solo si ningún campo quedó con error
        return string.IsNullOrEmpty(ErrorNombre)
            && string.IsNullOrEmpty(ErrorPrecio)
            && string.IsNullOrEmpty(ErrorDuracion);
    }

    private void LimpiarFormulario()
    {
        Nombre = string.Empty;
        Precio = string.Empty;
        Duracion = string.Empty;

        ErrorNombre = ErrorPrecio = ErrorDuracion = string.Empty;
    }
}