namespace ReservasBarberia.Models;

public class Reserva
{
    public int Id { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string ClienteTelefono { get; set; } = string.Empty;
    public string Barbero { get; set; } = string.Empty;
    public string Servicio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
}