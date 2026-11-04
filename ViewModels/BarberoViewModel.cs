using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReservasBarberia.Models;
using ReservasBarberia.Services;
using ReservasBarberia.Utilidades;

namespace ReservasBarberia.ViewModels;

public partial class BarberoViewModel : ObservableObject
{
    private readonly DataService _data;

    public BarberoViewModel(DataService data)
    {
        _data = data;
    }

    
    public ObservableCollection<Barbero> Barberos => _data.Barberos;

    
    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string especialidad = string.Empty;
    [ObservableProperty] private string telefono = string.Empty;

    
    [ObservableProperty] private string errorNombre = string.Empty;
    [ObservableProperty] private string errorEspecialidad = string.Empty;
    [ObservableProperty] private string errorTelefono = string.Empty;

    
    partial void OnNombreChanged(string value) => ErrorNombre = string.Empty;
    partial void OnEspecialidadChanged(string value) => ErrorEspecialidad = string.Empty;
    partial void OnTelefonoChanged(string value) => ErrorTelefono = string.Empty;

    
    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar())
            return;

        var barbero = new Barbero
        {
            Nombre = Nombre.Trim(),
            Especialidad = Especialidad.Trim(),
            Telefono = Validaciones.FormatearTelefono(Telefono)
        };

        _data.AgregarBarbero(barbero);

        await Shell.Current.DisplayAlert(
            "¡Barbero guardado!",
            $"{barbero.Nombre} ya forma parte del equipo.",
            "OK");

        LimpiarFormulario();
    }

    
    private bool Validar()
    {
        
        ErrorNombre = Validaciones.Nombre(Nombre, "del barbero");
        if (string.IsNullOrEmpty(ErrorNombre) &&
            _data.Barberos.Any(b => b.Nombre.Equals(Nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ErrorNombre = "Ya existe un barbero con ese nombre.";
        }

        
        string especialidadLimpia = Especialidad.Trim();
        if (especialidadLimpia.Length == 0)
            ErrorEspecialidad = "Escribe la especialidad del barbero.";
        else if (especialidadLimpia.Length < 4)
            ErrorEspecialidad = "La especialidad debe tener al menos 4 letras, por ejemplo: Fade y diseño.";
        else
            ErrorEspecialidad = string.Empty;

        
        ErrorTelefono = Validaciones.Telefono(Telefono, "del barbero");

        
        return string.IsNullOrEmpty(ErrorNombre)
            && string.IsNullOrEmpty(ErrorEspecialidad)
            && string.IsNullOrEmpty(ErrorTelefono);
    }

    private void LimpiarFormulario()
    {
        Nombre = string.Empty;
        Especialidad = string.Empty;
        Telefono = string.Empty;

        ErrorNombre = ErrorEspecialidad = ErrorTelefono = string.Empty;
    }
}