using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ReservasBarberia.Models;
using ReservasBarberia.Services;

namespace ReservasBarberia.ViewModels;

public partial class EstadisticasViewModel : ObservableObject
{
    private readonly DataService _data;

    public EstadisticasViewModel(DataService data)
    {
        _data = data;
    }

    public ObservableCollection<ConteoItem> ServiciosPopulares { get; } = new();
    public ObservableCollection<ConteoItem> ReservasPorBarbero { get; } = new();

    [ObservableProperty] private int totalReservas;
    [ObservableProperty] private int reservasHoy;
    [ObservableProperty] private string ingresoEstimado = "RD$0.00";
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SinDatos))]
    private bool hayDatos;

    public bool SinDatos => !HayDatos;


    public async Task CargarAsync()
    {
        await _data.InicializarAsync();

        TotalReservas = await _data.ContarReservasAsync();
        ReservasHoy = await _data.ContarReservasDeHoyAsync();

        decimal ingreso = await _data.IngresoEstimadoAsync();
        IngresoEstimado = $"RD${ingreso:N2}";

        HayDatos = TotalReservas > 0;

        Llenar(ServiciosPopulares, await _data.ServiciosMasPedidosAsync());
        Llenar(ReservasPorBarbero, await _data.ReservasPorBarberoAsync());
    }

    
    private static void Llenar(ObservableCollection<ConteoItem> destino, List<ConteoItem> datos)
    {
        int maximo = datos.Count > 0 ? datos.Max(d => d.Total) : 0;

        destino.Clear();

        foreach (var dato in datos)
        {
            dato.Proporcion = maximo == 0 ? 0 : (double)dato.Total / maximo;
            destino.Add(dato);
        }
    }
}