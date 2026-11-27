namespace ReservasBarberia.Services;

public class RegistroDuplicadoException : Exception
{
    public string Campo { get; }

    public RegistroDuplicadoException(string campo, string mensaje)
        : base(mensaje)
    {
        Campo = campo;
    }
}