using ReservasBarberia.Models;
using ReservasBarberia.Services;

namespace ReservasBarberia.Views;

public partial class ServiciosPage : ContentPage
{
    private readonly DataService _data;

    public ServiciosPage(DataService data)
    {
        InitializeComponent();
        _data = data;

        BindableLayout.SetItemsSource(ListaServicios, _data.Servicios);
    }

    private async void OnGuardarClicked(object sender, EventArgs e)
    {
        string nombre = NombreEntry.Text?.Trim() ?? string.Empty;
        string precioTexto = PrecioEntry.Text?.Trim() ?? string.Empty;
        string duracionTexto = DuracionEntry.Text?.Trim() ?? string.Empty;

        
        if (string.IsNullOrWhiteSpace(nombre))
        {
            MostrarError("El nombre del servicio es obligatorio.");
            return;
        }

        if (string.IsNullOrWhiteSpace(precioTexto))
        {
            MostrarError("El precio es obligatorio.");
            return;
        }

        
        if (!decimal.TryParse(precioTexto, out decimal precio) || precio <= 0)
        {
            MostrarError("El precio debe ser un número mayor que cero.");
            return;
        }

        if (string.IsNullOrWhiteSpace(duracionTexto))
        {
            MostrarError("La duración es obligatoria.");
            return;
        }

        
        if (!int.TryParse(duracionTexto, out int duracion) || duracion <= 0)
        {
            MostrarError("La duración debe ser un número entero mayor que cero.");
            return;
        }

        var servicio = new Servicio
        {
            Nombre = nombre,
            Precio = precio,
            DuracionMinutos = duracion
        };

        _data.AgregarServicio(servicio);

        LimpiarFormulario();
        await DisplayAlert("Listo", "El servicio se guardó correctamente.", "OK");
    }

    private void MostrarError(string mensaje)
    {
        MensajeError.Text = mensaje;
        MensajeError.IsVisible = true;
    }

    private void LimpiarFormulario()
    {
        NombreEntry.Text = string.Empty;
        PrecioEntry.Text = string.Empty;
        DuracionEntry.Text = string.Empty;
        MensajeError.IsVisible = false;
    }
}