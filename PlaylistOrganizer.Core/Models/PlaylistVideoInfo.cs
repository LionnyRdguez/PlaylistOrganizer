namespace PlaylistOrganizer.Core.Models;

// Representa la información mínima de un video dentro de una playlist de YouTube.

public sealed class PlaylistVideoInfo
{
    // Posición del video dentro de la playlist
    // Se usa para construir el prefijo numérico al renombrar.
    
    public required int Index { get; init; }

    // Identificador único del video en YouTube 
    public required string VideoId { get; init; }
   
    // Título original del video tal como aparece en YouTube.
    // Es la referencia contra la que se compararán los nombres de archivo locales.
    public required string Title { get; init; }

    // Duración del video, si está disponible.  
    public TimeSpan? Duration { get; init; }
}