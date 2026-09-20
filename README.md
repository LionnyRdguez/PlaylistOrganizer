# Playlist Organizer

> Cross-platform tool to organize locally downloaded video/audio files using the original order of a YouTube playlist.

## 📋 Description

**Playlist Organizer** solves the problem of messy downloaded files: you provide the URL of a YouTube playlist and the local folder where your files are stored, and the application:

1. Extracts the original video order from the playlist.
2. Compares YouTube titles with local file names using string similarity.
3. Renames the files on disk by adding a numeric prefix with zero-padding (e.g. `001 - Video title.mp4`).

## 🖥️ Supported platforms

- Windows
- Linux
- macOS
- Android

## 🛠️ Tech stack

- **Language:** C# / .NET 8
- **UI:** Avalonia UI (v11+)
- **YouTube metadata:** YoutubeExplode
- **Matching algorithm:** Levenshtein distance
- **Architecture:** Clean Architecture + MVVM

## 🚧 Project status

In design phase. Solution structure and first services coming soon.

## 📄 License

This project is licensed under the [MIT License](LICENSE). You are free to use, modify, and distribute it as long as you include the original copyright notice.
