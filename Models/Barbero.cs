using SQLite;
namespace ReservasBarberia.Models;

[Table("Barberos")]
public class Barbero
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Unique, Collation("NOCASE")]
    public string Nombre { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}