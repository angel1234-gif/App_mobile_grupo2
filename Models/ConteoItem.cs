using SQLite;

namespace ReservasBarberia.Models;


public class ConteoItem
{
    public string Nombre { get; set; } = string.Empty;
    public int Total { get; set; }

    
    [Ignore]
    public double Proporcion { get; set; }
}