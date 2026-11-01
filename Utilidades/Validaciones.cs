namespace ReservasBarberia.Utilidades;

// Reglas de validación reutilizables. Cada método devuelve el mensaje de error,
// o un texto vacío si el valor es válido.
public static class Validaciones
{
    public static string SoloDigitos(string texto)
    {
        return new string(texto.Where(char.IsDigit).ToArray());
    }

    
    public static string Nombre(string valor, string de)
    {
        string nombre = valor.Trim();

        if (nombre.Length == 0)
            return $"Escribe el nombre {de}.";
        if (nombre.Length < 3)
            return "El nombre debe tener al menos 3 letras.";
        if (nombre.Any(char.IsDigit))
            return "El nombre no puede contener números.";

        return string.Empty;
    }

    // Formato dominicano: 10 dígitos que empiezan con 809, 829 o 849
    public static string Telefono(string valor, string de)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return $"Escribe el teléfono {de}.";
        if (valor.Any(char.IsLetter))
            return "El teléfono solo puede tener números.";

        string digitos = SoloDigitos(valor);

        if (digitos.Length != 10)
            return "El teléfono debe tener 10 dígitos, por ejemplo 809-555-1234.";
        if (!(digitos.StartsWith("809") || digitos.StartsWith("829") || digitos.StartsWith("849")))
            return "El teléfono debe empezar con 809, 829 o 849.";

        return string.Empty;
    }

    // Convierte 8095551234 en 809-555-1234
    public static string FormatearTelefono(string valor)
    {
        string d = SoloDigitos(valor);
        return $"{d[..3]}-{d[3..6]}-{d[6..]}";
    }
}