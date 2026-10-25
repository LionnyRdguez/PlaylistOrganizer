using FluentAssertions;
using PlaylistOrganizer.Core.Services;
using YoutubeExplode.Playlists;

namespace PlaylistOrganizer.Tests.Services;

/// <summary>
/// Tests para la lógica de validación y parseo de URLs de <see cref="YoutubeFetcherService"/>.
/// </summary>
public class YoutubeFetcherServiceTests
{
    [Theory]
    // URLs válidas
    [InlineData("https://www.youtube.com/playlist?list=PLBCF2DAC6FFB574DE")]
    [InlineData("https://youtube.com/playlist?list=PLBCF2DAC6FFB574DE")]
    [InlineData("https://music.youtube.com/playlist?list=PLBCF2DAC6FFB574DE")]
    public void TryParsePlaylistId_WithValidUrls_ShouldReturnTrue(string url)
    {
        // Act
        var result = YoutubeFetcherService.TryParsePlaylistId(url, out var playlistId);

        // Assert
        result.Should().BeTrue();
        playlistId.Value.Should().Be("PLBCF2DAC6FFB574DE");
    }

    [Theory]
    // URLs inválidas
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ")] // Es un video, no una playlist
    [InlineData("https://www.google.com")]
    [InlineData("not a url")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParsePlaylistId_WithInvalidUrls_ShouldReturnFalse(string? url)
    {
        // Act
        var result = YoutubeFetcherService.TryParsePlaylistId(url!, out var playlistId);

        // Assert
        result.Should().BeFalse();

        // Nota: PlaylistId es un struct, no puede ser null. 
        // Comprobamos que su propiedad Value (que sí es un string) sea null.
        playlistId.Value.Should().BeNull();
    }
}