using SQLite;
namespace ReservasBarberia.Models;

[Table("Reservas")]
public class Reserva
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string ClienteTelefono { get; set; } = string.Empty;
    [Unique(Name = "ux_reserva_horario", Order = 1)]
    public string Barbero { get; set; } = string.Empty;
    public string Servicio { get; set; } = string.Empty;
    [Unique(Name = "ux_reserva_horario", Order = 2)]
    public DateTime Fecha { get; set; }
    [Unique(Name = "ux_reserva_horario", Order = 3)]
    public TimeSpan Hora { get; set; }
    [Ignore]
    public bool Confirmada { get; set; }
    public string FechaHoraTexto => $"{Fecha:dd/MM/yyyy} - {Hora:hh\\:mm}";
}