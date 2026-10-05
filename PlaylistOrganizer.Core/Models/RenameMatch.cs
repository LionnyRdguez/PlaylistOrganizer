namespace PlaylistOrganizer.Core.Models;

//Resultado del emparejamiento entre un archivo local y un video de la playlist. 
//Contiene toda la información necesaria para previsualizar o ejecutar un renombrado.

public sealed class RenameMatch
{
    // Ruta absoluta actual del archivo en disco.
    public required string OriginalPath { get; init; }

    // Ruta absoluta que tendrá el archivo tras el renombrado.
    // Se precalcula para que el usuario pueda previsualizarla antes de aplicar.
    public required string NewFullPath { get; init; }

    // Índice del video en la playlist que se le asignó (base 1).
    public required int PlaylistIndex { get; init; }

    // Título del video de YouTube con el que se emparejó.
    public required string MatchedVideoTitle { get; init; }

    // Puntaje de similitud entre 0.0 y 1.0.
    // 1.0 = coincidencia exacta, 0.0 = nada en común.
    public required double SimilarityScore { get; init; }

    // Indica si el puntaje supera el umbral mínimo aceptable definido por el matcher.
    // Las coincidencias deberían mostrarse al usuario para confirmación manual.
    public bool IsConfident => SimilarityScore >= ConfidenceThreshold;

    // Umbral de confianza por encima del cual consideramos una coincidencia fiable.
    // 0.75 significa que el 75% del contenido coincide.
    public const double ConfidenceThreshold = 0.75;
}