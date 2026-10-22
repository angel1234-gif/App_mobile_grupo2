using ReservasBarberia.Models;
using ReservasBarberia.Services;

namespace ReservasBarberia.Views;

public partial class BarberosPage : ContentPage
{
    private readonly DataService _data;

    public BarberosPage(DataService data)
    {
        InitializeComponent();
        _data = data;

        // La lista de la pantalla lee directamente de la libreta compartida
        BindableLayout.SetItemsSource(ListaBarberos, _data.Barberos);
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        string nombre = NombreEntry.Text?.Trim() ?? string.Empty;
        string especialidad = EspecialidadEntry.Text?.Trim() ?? string.Empty;
        string telefono = TelefonoEntry.Text?.Trim() ?? string.Empty;

        
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarError("El nombre es obligatorio.");
            return;
        }

        if (string.IsNullOrWhiteSpace(especialidad))
        {
            MostrarError("La especialidad es obligatoria.");
            return;
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            MostrarError("El teléfono es obligatorio.");
            return;
        }

        var barbero = new Barbero
        {
            Nombre = nombre,
            Especialidad = especialidad,
            Telefono = telefono
        };

        _data.AgregarBarbero(barbero);

        LimpiarFormulario();
        await DisplayAlert("Listo", "El barbero se guardó correctamente.", "OK");
    }

    private void MostrarError(string mensaje)
    {
        MensajeError.Text = mensaje;
        MensajeError.IsVisible = true;
    }

    private void LimpiarFormulario()
    {
        NombreEntry.Text = string.Empty;
        EspecialidadEntry.Text = string.Empty;
        TelefonoEntry.Text = string.Empty;
        MensajeError.IsVisible = false;
    }
}