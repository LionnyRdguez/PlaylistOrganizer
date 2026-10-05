namespace PlaylistOrganizer.Core.Models;

// Opciones de configuración para el proceso de renombrado.
// Permite que la UI exponga estas opciones al usuario.
public sealed class RenameOptions
{
    // Número mínimo de dígitos del prefijo numérico.
    // Se calculará automáticamente a partir del tamaño de la playlist si no se especifica.
    public int Padding { get; init; } = 2;


    // Separador entre el prefijo numérico y el título del video.
    // Por defecto " - " produce nombres como "001 - Video title.mp4".
    public string Separator { get; init; } = " - ";

    // Caracteres prohibidos en el sistema de archivos. Se sustituyen por '_'.
    // Incluye los inválidos en Windows, macOS y Linux, además de los caracteres de control
    public char[] InvalidChars { get; init; } = DefaultInvalidChars();

    // Si es true, el servicio solo simula los cambios sin tocar el disco.
    // Esencial para que la UI muestre una previsualización antes de aplicar.
    public bool DryRun { get; init; } = true;

    // Devuelve la lista por defecto de caracteres inválidos.
    // Se genera en un método para evitar arrays mutables compartidos.
    private static char[] DefaultInvalidChars() =>
    [
        '<', '>', ':', '"', '/', '\\', '|', '?', '*'
    ];
}