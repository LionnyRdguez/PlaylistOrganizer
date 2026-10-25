using Microsoft.Extensions.DependencyInjection;
using PlaylistOrganizer.Core.Services.Abstractions;
using YoutubeExplode;

namespace PlaylistOrganizer.Core.Services;

/// <summary>
/// Extensiones para registrar los servicios del dominio en el contenedor de DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos los servicios de la capa Core.
    /// </summary>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Registramos el cliente de YoutubeExplode como Singleton (es thread-safe y reutiliza conexiones).
        services.AddSingleton<YoutubeClient>();

        // Registramos nuestro servicio como Transient.
        services.AddTransient<IYoutubeFetcherService, YoutubeFetcherService>();

        return services;
    }
}