using ReservasBarberia.Models;
using ReservasBarberia.Services;

namespace ReservasBarberia.Views;

public partial class FormularioReservaPage : ContentPage
{
    private readonly DataService _data;

    
    public FormularioReservaPage(DataService data)
    {
        InitializeComponent();
        _data = data;
    }

    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        BarberoPicker.ItemsSource = _data.Barberos.Select(b => b.Nombre).ToList();
        ServicioPicker.ItemsSource = _data.Servicios.Select(s => s.Nombre).ToList();
        FechaPicker.MinimumDate = DateTime.Today;
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        string nombre = NombreEntry.Text?.Trim() ?? string.Empty;
        string telefono = TelefonoEntry.Text?.Trim() ?? string.Empty;

        
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarError("El nombre del cliente es obligatorio.");
            return;
        }

        if (string.IsNullOrWhiteSpace(telefono))
        {
            MostrarError("El teléfono es obligatorio.");
            return;
        }

        if (BarberoPicker.SelectedIndex < 0)
        {
            MostrarError("Selecciona un barbero.");
            return;
        }

        if (ServicioPicker.SelectedIndex < 0)
        {
            MostrarError("Selecciona un servicio.");
            return;
        }

        
        var reserva = new Reserva
        {
            ClienteNombre = nombre,
            ClienteTelefono = telefono,
            Barbero = BarberoPicker.SelectedItem?.ToString() ?? string.Empty,
            Servicio = ServicioPicker.SelectedItem?.ToString() ?? string.Empty,
            Fecha = FechaPicker.Date,
            Hora = HoraPicker.Time
        };

        _data.AgregarReserva(reserva);

        LimpiarFormulario();
        await DisplayAlert("Listo", "La reserva se guardó correctamente.", "OK");
    }

    private void MostrarError(string mensaje)
    {
        MensajeError.Text = mensaje;
        MensajeError.IsVisible = true;
    }

    private void LimpiarFormulario()
    {
        NombreEntry.Text = string.Empty;
        TelefonoEntry.Text = string.Empty;
        BarberoPicker.SelectedIndex = -1;
        ServicioPicker.SelectedIndex = -1;
        FechaPicker.Date = DateTime.Today;
        HoraPicker.Time = new TimeSpan(9, 0, 0);
        MensajeError.IsVisible = false;
    }
}