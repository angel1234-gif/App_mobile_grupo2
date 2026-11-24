using SQLite;
namespace ReservasBarberia.Models;

[Table("Clientes")]
public class Cliente
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    [Unique]
    public string Telefono { get; set; } = string.Empty;
    [Unique, Collation("NOCASE")]
    public string Correo { get; set; } = string.Empty;
}