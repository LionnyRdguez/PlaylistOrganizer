using PlaylistOrganizer.Core.Errors;
using PlaylistOrganizer.Core.Models;
using PlaylistOrganizer.Core.Results;
using PlaylistOrganizer.Core.Services.Abstractions;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;

namespace PlaylistOrganizer.Core.Services;

/// <summary>
/// Implementación de <see cref="IYoutubeFetcherService"/> usando la librería YoutubeExplode.
/// </summary>
public class YoutubeFetcherService : IYoutubeFetcherService
{
    private readonly YoutubeClient _youtubeClient;

    /// <summary>
    /// Constructor. Inyectamos el cliente de YouTube para facilitar pruebas de integración futuras.
    /// </summary>
    public YoutubeFetcherService(YoutubeClient youtubeClient)
    {
        _youtubeClient = youtubeClient ?? throw new ArgumentNullException(nameof(youtubeClient));
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<PlaylistVideoInfo>>> GetPlaylistAsync(
        string url,
        CancellationToken cancellationToken = default)
    {
        // 1. Validación temprana: parseamos la URL antes de tocar la red.
        if (!TryParsePlaylistId(url, out var playlistId))
        {
            return Result<IReadOnlyList<PlaylistVideoInfo>>.Failure(AppError.InvalidUrl(url));
        }

        try
        {
            // 2. Llamada a la API de YouTube.
            // GetVideosAsync devuelve un IAsyncEnumerable, lo iteramos para construir la lista.
            var videos = new List<PlaylistVideoInfo>();
            var index = 1; // YouTube usa base 1 para el orden visual.

            await foreach (var video in _youtubeClient.Playlists.GetVideosAsync(playlistId, cancellationToken))
            {
                videos.Add(new PlaylistVideoInfo
                {
                    Index = index++,
                    VideoId = video.Id.Value,
                    Title = video.Title,
                    Duration = video.Duration
                });
            }

            // 3. Validación post-red: ¿la playlist estaba vacía?
            if (videos.Count == 0)
            {
                return Result<IReadOnlyList<PlaylistVideoInfo>>.Failure(AppError.EmptyPlaylist());
            }

            return Result<IReadOnlyList<PlaylistVideoInfo>>.Success(videos);
        }
        // 4. Mapeo de excepciones a AppError
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Result<IReadOnlyList<PlaylistVideoInfo>>.Failure(AppError.CancelledOperation());
        }
        catch (HttpRequestException ex)
        {
            return Result<IReadOnlyList<PlaylistVideoInfo>>.Failure(
                AppError.NetworkFailure("Failed to connect to YouTube.", ex));
        }
        catch (Exception ex)
        {
            // Fallback para excepciones de YoutubeExplode (video privado, geo-bloqueo, etc.)
            return Result<IReadOnlyList<PlaylistVideoInfo>>.Failure(
                AppError.YoutubeApiError("Error fetching playlist from YouTube.", ex));
        }
    }

    /// <summary>
    /// Intenta extraer el ID de la playlist de una URL.
    /// Se expone como público estático para poder ser testeado sin depender de la red.
    /// </summary>
    public static bool TryParsePlaylistId(string url, out PlaylistId playlistId)
    {
        // Devuelve el ID o null si no pudo parsear.
        var parsed = PlaylistId.TryParse(url);

        if (parsed is not null)
        {
            playlistId = parsed.Value;
            return true;
        }

        // Si falla, asignamos el valor por defecto obligatorio para 'out'
        playlistId = default!;
        return false;
    }
}