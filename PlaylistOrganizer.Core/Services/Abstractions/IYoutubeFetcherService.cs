using PlaylistOrganizer.Core.Errors;
using PlaylistOrganizer.Core.Models;
using PlaylistOrganizer.Core.Results;

namespace PlaylistOrganizer.Core.Services.Abstractions;

/// <summary>
/// Contrato para el servicio que extrae la información de una playlist de YouTube.
/// </summary>
public interface IYoutubeFetcherService
{
    /// <summary>
    /// Obtiene la lista de videos de una playlist a partir de su URL.
    /// </summary>
    /// <param name="url">La URL de la playlist de YouTube.</param>
    /// <param name="cancellationToken">Token para cancelar la operación asíncrona.</param>
    /// <returns>
    /// Un <see cref="Result{T}"/> con la lista de videos, o un error tipado
    /// (<see cref="ErrorCode.InvalidUrl"/>, <see cref="ErrorCode.NetworkFailure"/>, etc.).
    /// </returns>
    Task<Result<IReadOnlyList<PlaylistVideoInfo>>> GetPlaylistAsync(
        string url,
        CancellationToken cancellationToken = default);
}