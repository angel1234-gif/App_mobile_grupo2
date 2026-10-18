using System.Collections.ObjectModel;
using ReservasBarberia.Models;

namespace ReservasBarberia.Services;

public class DataService
{
 
    public ObservableCollection<Barbero> Barberos { get; } = new()
    {
        new Barbero { Id = 1, Nombre = "Junior Pérez", Especialidad = "Fade y diseño", Telefono = "809-000-0001" },
        new Barbero { Id = 2, Nombre = "Carlos Reyes", Especialidad = "Clásico y barba", Telefono = "809-000-0002" },
    };

    public ObservableCollection<Servicio> Servicios { get; } = new()
    {
        new Servicio { Id = 1, Nombre = "Corte clásico", Precio = 300, DuracionMinutos = 30 },
        new Servicio { Id = 2, Nombre = "Corte + barba", Precio = 500, DuracionMinutos = 45 },
    };

    public ObservableCollection<Cliente> Clientes { get; } = new();

    public ObservableCollection<Reserva> Reservas { get; } = new();

    
    public void AgregarBarbero(Barbero barbero)
    {
        barbero.Id = Barberos.Count == 0 ? 1 : Barberos.Max(b => b.Id) + 1;
        Barberos.Add(barbero);
    }

    public void AgregarServicio(Servicio servicio)
    {
        servicio.Id = Servicios.Count == 0 ? 1 : Servicios.Max(s => s.Id) + 1;
        Servicios.Add(servicio);
    }

    public void AgregarCliente(Cliente cliente)
    {
        cliente.Id = Clientes.Count == 0 ? 1 : Clientes.Max(c => c.Id) + 1;
        Clientes.Add(cliente);
    }

    public void AgregarReserva(Reserva reserva)
    {
        reserva.Id = Reservas.Count == 0 ? 1 : Reservas.Max(r => r.Id) + 1;
        Reservas.Add(reserva);
    }
}