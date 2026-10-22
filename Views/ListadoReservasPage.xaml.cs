using ReservasBarberia.Services;

namespace ReservasBarberia.Views;

public partial class ListadoReservasPage : ContentPage
{
    public ListadoReservasPage(DataService data)
    {
        InitializeComponent();

        
        ReservasCollection.ItemsSource = data.Reservas;
    }
}