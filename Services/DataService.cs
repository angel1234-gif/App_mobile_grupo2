using System.Collections.ObjectModel;
using Microsoft.Maui.Storage;
using ReservasBarberia.Models;
using SQLite;

namespace ReservasBarberia.Services;

public class DataService
{
    private const string NombreArchivo = "barberia.db3";
    private const string ClaveSembrado = "bd_sembrada_v1";

    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    private SQLiteAsyncConnection? _db;
    private bool _inicializado;
    private readonly SemaphoreSlim _candado = new(1, 1);

    public ObservableCollection<Barbero> Barberos { get; } = new();
    public ObservableCollection<Servicio> Servicios { get; } = new();
    public ObservableCollection<Cliente> Clientes { get; } = new();
    public ObservableCollection<Reserva> Reservas { get; } = new();

    public Reserva? ReservaPorEditar { get; set; }

    private SQLiteAsyncConnection Db =>
        _db ?? throw new InvalidOperationException(
            "La base de datos no está inicializada. Llama a InicializarAsync().");

    public async Task InicializarAsync()
    {
        if (_inicializado)
            return;

        await _candado.WaitAsync();

        try
        {
            if (_inicializado)
                return;

            string ruta = Path.Combine(
                FileSystem.AppDataDirectory,
                NombreArchivo);

            _db = new SQLiteAsyncConnection(ruta, Flags);

            await _db.CreateTableAsync<Barbero>();
            await _db.CreateTableAsync<Servicio>();
            await _db.CreateTableAsync<Cliente>();
            await _db.CreateTableAsync<Reserva>();

            if (!Preferences.Default.Get(ClaveSembrado, false))
            {
                await SembrarAsync();
                Preferences.Default.Set(ClaveSembrado, true);
            }

            await CargarTodoAsync();

            _inicializado = true;
        }
        finally
        {
            _candado.Release();
        }
    }

    private async Task SembrarAsync()
    {
        await Db.InsertAllAsync(new List<Barbero>
        {
            new()
            {
                Nombre = "Junior Pérez",Especialidad = "Fade y diseño",Telefono = "809-000-0001"},
            new()
            {
                Nombre = "Carlos Reyes",Especialidad = "Clásico y barba",Telefono = "809-000-0002"}
        });

        await Db.InsertAllAsync(new List<Servicio>
        {
            new()
            {
                Nombre = "Corte clásico",
                Precio = 300,
                DuracionMinutos = 30
            },
            new()
            {
                Nombre = "Corte + barba",
                Precio = 500,
                DuracionMinutos = 45
            }
        });
    }

    private async Task CargarTodoAsync()
    {
        Reemplazar(
            Barberos,
            await Db.Table<Barbero>().ToListAsync());

        Reemplazar(
            Servicios,
            await Db.Table<Servicio>().ToListAsync());

        Reemplazar(
            Clientes,
            await Db.Table<Cliente>().ToListAsync());

        var reservas = await Db.Table<Reserva>().ToListAsync();

        Reemplazar(
            Reservas,
            reservas.OrderBy(r => r.Fecha)
                    .ThenBy(r => r.Hora));
    }

    private static void Reemplazar<T>(
        ObservableCollection<T> coleccion,
        IEnumerable<T> nuevos)
    {
        coleccion.Clear();

        foreach (var item in nuevos)
            coleccion.Add(item);
    }

    private static void Sustituir<T>(
        ObservableCollection<T> coleccion,
        Func<T, bool> coincide,
        T nuevo)
    {
        var actual = coleccion.FirstOrDefault(coincide);

        if (actual is not null)
            coleccion[coleccion.IndexOf(actual)] = nuevo;
    }

    private void OrdenarReservas()
    {
        var ordenadas = Reservas.OrderBy(r => r.Fecha).ThenBy(r => r.Hora).ToList();

        for (int i = 0; i < ordenadas.Count; i++)
        {
            int actual = Reservas.IndexOf(ordenadas[i]);
            if (actual != i)
                Reservas.Move(actual, i);
        }
    }

    private static async Task EjecutarAsync(Func<Task> operacion)
    {
        try
        {
            await operacion();
        }
        catch (SQLiteException ex)
            when (ex.Result == SQLite3.Result.Constraint)
        {
            string campo = string.Empty;

            int i = ex.Message.IndexOf(':');

            if (i >= 0)
            {
                string primera = ex.Message[(i + 1)..]
                    .Split(',')[0].Trim();

                campo = primera[
                    (primera.LastIndexOf('.') + 1)..];
            }

            throw new RegistroDuplicadoException(campo, ex.Message);
        }
    }

    public async Task AgregarBarberoAsync(Barbero barbero)
    {
        await EjecutarAsync(() => Db.InsertAsync(barbero));
        Barberos.Add(barbero);
    }

    public async Task AgregarServicioAsync(Servicio servicio)
    {
        await EjecutarAsync(() => Db.InsertAsync(servicio));
        Servicios.Add(servicio);
    }

    public async Task AgregarClienteAsync(Cliente cliente)
    {
        await EjecutarAsync(() => Db.InsertAsync(cliente));
        Clientes.Add(cliente);
    }

    public async Task AgregarReservaAsync(Reserva reserva)
    {
        await EjecutarAsync(() => Db.InsertAsync(reserva));
        Reservas.Add(reserva);
        OrdenarReservas();
    }

    public async Task ActualizarBarberoAsync(Barbero editado)
    {
        await EjecutarAsync(() => Db.UpdateAsync(editado));

        Sustituir(
            Barberos,
            b => b.Id == editado.Id,
            editado);
    }

    public async Task ActualizarServicioAsync(Servicio editado)
    {
        await EjecutarAsync(() => Db.UpdateAsync(editado));

        Sustituir(
            Servicios,
            s => s.Id == editado.Id,
            editado);
    }

    public async Task ActualizarClienteAsync(Cliente editado)
    {
        await EjecutarAsync(() => Db.UpdateAsync(editado));

        Sustituir(
            Clientes,
            c => c.Id == editado.Id,
            editado);
    }

    public async Task ActualizarReservaAsync(Reserva editada)
    {
        await EjecutarAsync(() => Db.UpdateAsync(editada));
        Sustituir(Reservas, r => r.Id == editada.Id, editada);
        OrdenarReservas();
    }

    public async Task EliminarBarberoAsync(Barbero barbero)
    {
        await Db.DeleteAsync(barbero);

        var actual = Barberos.FirstOrDefault(
            b => b.Id == barbero.Id);

        if (actual is not null)
            Barberos.Remove(actual);
    }

    public async Task EliminarServicioAsync(Servicio servicio)
    {
        await Db.DeleteAsync(servicio);

        var actual = Servicios.FirstOrDefault(
            s => s.Id == servicio.Id);

        if (actual is not null)
            Servicios.Remove(actual);
    }

    public async Task EliminarClienteAsync(Cliente cliente)
    {
        await Db.DeleteAsync(cliente);

        var actual = Clientes.FirstOrDefault(
            c => c.Id == cliente.Id);

        if (actual is not null)
            Clientes.Remove(actual);
    }

    public async Task EliminarReservaAsync(Reserva reserva)
    {
        await Db.DeleteAsync(reserva);

        var actual = Reservas.FirstOrDefault(
            r => r.Id == reserva.Id);

        if (actual is not null)
            Reservas.Remove(actual);
    }

    

    public async Task<int> ContarReservasAsync()
    {
        return await Db.Table<Reserva>().CountAsync();
    }

    public async Task<int> ContarReservasDeHoyAsync()
    {
        DateTime hoy = DateTime.Today;
        return await Db.Table<Reserva>().Where(r => r.Fecha == hoy).CountAsync();
    }

    
    public async Task<decimal> IngresoEstimadoAsync()
    {
        double total = await Db.ExecuteScalarAsync<double>(
            "SELECT IFNULL(SUM(s.Precio), 0) " +
            "FROM Reservas r INNER JOIN Servicios s ON s.Nombre = r.Servicio");

        return (decimal)total;
    }

    public Task<List<ConteoItem>> ServiciosMasPedidosAsync()
    {
        return Db.QueryAsync<ConteoItem>(
            "SELECT Servicio AS Nombre, COUNT(*) AS Total " +
            "FROM Reservas GROUP BY Servicio ORDER BY Total DESC");
    }

    public Task<List<ConteoItem>> ReservasPorBarberoAsync()
    {
        return Db.QueryAsync<ConteoItem>(
            "SELECT Barbero AS Nombre, COUNT(*) AS Total " +
            "FROM Reservas GROUP BY Barbero ORDER BY Total DESC");
    }
}