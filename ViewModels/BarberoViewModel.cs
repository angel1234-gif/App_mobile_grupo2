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

    
    public event Action? IrAlFormulario;

    
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsEdicion))]
    [NotifyPropertyChangedFor(nameof(Titulo))]
    [NotifyPropertyChangedFor(nameof(TextoBotonGuardar))]
    private int? idEnEdicion;

    public bool EsEdicion => IdEnEdicion.HasValue;
    public string Titulo => EsEdicion ? "Editar barbero" : "Nuevo barbero";
    public string TextoBotonGuardar => EsEdicion ? "Guardar cambios" : "Guardar barbero";

    
    [ObservableProperty] private string nombre = string.Empty;
    [ObservableProperty] private string especialidad = string.Empty;
    [ObservableProperty] private string telefono = string.Empty;

    
    [ObservableProperty] private string errorNombre = string.Empty;
    [ObservableProperty] private string errorEspecialidad = string.Empty;
    [ObservableProperty] private string errorTelefono = string.Empty;

    
    partial void OnNombreChanged(string value) => ErrorNombre = string.Empty;
    partial void OnEspecialidadChanged(string value) => ErrorEspecialidad = string.Empty;
    partial void OnTelefonoChanged(string value) => ErrorTelefono = string.Empty;

    
    public void AlMostrarPantalla() => LimpiarFormulario();

    
    [RelayCommand]
    private void Editar(Barbero barbero)
    {
        if (barbero is null)
            return;

        IdEnEdicion = barbero.Id;
        Nombre = barbero.Nombre;
        Especialidad = barbero.Especialidad;
        Telefono = barbero.Telefono;

        LimpiarErrores();
        IrAlFormulario?.Invoke();
    }

    
    [RelayCommand]
    private async Task GuardarAsync()
    {
        if (!Validar())
            return;

        var barbero = new Barbero
        {
            Id = IdEnEdicion ?? 0, 
            Nombre = Nombre.Trim(),
            Especialidad = Especialidad.Trim(),
            Telefono = Validaciones.FormatearTelefono(Telefono)
        };

        if (EsEdicion)
        {
            _data.ActualizarBarbero(barbero);

            await Shell.Current.DisplayAlert(
                "¡Cambios guardados!",
                $"Los datos de {barbero.Nombre} se actualizaron.",
                "OK");
        }
        else
        {
            _data.AgregarBarbero(barbero);

            await Shell.Current.DisplayAlert(
                "¡Barbero guardado!",
                $"{barbero.Nombre} ya forma parte del equipo.",
                "OK");
        }

        LimpiarFormulario();
    }

    
    [RelayCommand]
    private void CancelarEdicion() => LimpiarFormulario();

    
    [RelayCommand]
    private async Task EliminarDeListaAsync(Barbero barbero)
    {
        if (barbero is null)
            return;

        if (await ConfirmarYEliminarAsync(barbero) && IdEnEdicion == barbero.Id)
            LimpiarFormulario(); // si justo se estaba editando, se sale del modo edición
    }

    
    [RelayCommand]
    private async Task EliminarActualAsync()
    {
        var barbero = _data.Barberos.FirstOrDefault(b => b.Id == IdEnEdicion);
        if (barbero is null)
            return;

        if (await ConfirmarYEliminarAsync(barbero))
            LimpiarFormulario();
    }

    
    private async Task<bool> ConfirmarYEliminarAsync(Barbero barbero)
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Eliminar barbero",
            $"¿Seguro que quieres eliminar a {barbero.Nombre}? Esta acción no se puede deshacer.",
            "Sí, eliminar",
            "Cancelar");

        if (!confirmar)
            return false;

        _data.EliminarBarbero(barbero);
        return true;
    }

    
    private bool Validar()
    {
        LimpiarErrores();

        
        ErrorNombre = Validaciones.Nombre(Nombre, "del barbero");
        if (string.IsNullOrEmpty(ErrorNombre) &&
            _data.Barberos.Any(b => b.Id != IdEnEdicion &&
                                    b.Nombre.Equals(Nombre.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            ErrorNombre = "Ya existe un barbero con ese nombre.";
        }

        
        string especialidadLimpia = Especialidad.Trim();
        if (especialidadLimpia.Length == 0)
            ErrorEspecialidad = "Escribe la especialidad del barbero.";
        else if (especialidadLimpia.Length < 4)
            ErrorEspecialidad = "La especialidad debe tener al menos 4 letras, por ejemplo: Fade y diseño.";

        
        ErrorTelefono = Validaciones.Telefono(Telefono, "del barbero");

        return string.IsNullOrEmpty(ErrorNombre)
            && string.IsNullOrEmpty(ErrorEspecialidad)
            && string.IsNullOrEmpty(ErrorTelefono);
    }

    private void LimpiarErrores()
    {
        ErrorNombre = ErrorEspecialidad = ErrorTelefono = string.Empty;
    }

    
    private void LimpiarFormulario()
    {
        IdEnEdicion = null;
        Nombre = string.Empty;
        Especialidad = string.Empty;
        Telefono = string.Empty;

        LimpiarErrores();
    }
}