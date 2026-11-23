using SQLite;
namespace ReservasBarberia.Models;

[Table("Servicios")]
public class Servicio
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Unique, Collation("NOCASE")]
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public int DuracionMinutos { get; set; }
}