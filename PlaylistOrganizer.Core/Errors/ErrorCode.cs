namespace PlaylistOrganizer.Core.Errors
{
    public enum ErrorCode
    {
        None = 0,
       
        // Errores de entrada / validación
        InvalidUrl = 1,
        EmptyPlaylist = 2,
        EmptyFolder = 3,

        // Errores de red
        NetworkFailure = 4,
        PlaylistNotFound = 5,
        YoutubeApiError = 6,

        // Errores de sistema de archivos
        FileNotFound = 7,
        AccessDenied = 8,
        IoFailure = 9,
        CollisionDetected = 10,
        InvalidFileName = 11,

        // Errores de operación
        CancelledOperation = 12,


        UnknownError = 99 //Error inesperado que no encaja en ninguna otra categoría.

    }
}
