namespace PlaylistOrganizer.Core.Errors
{

    public sealed class AppError : IEquatable<AppError>
    {
        public ErrorCode Code { get; } // Código semántico que la UI puede usar para decidir cómo reaccionar.

        public string? Message { get; } // Mensaje técnico pensado para logs. No se muestra directamente al usuario final.

        public Exception? Exception { get; } // Excepción original que originó el error, si existe.

        private AppError(ErrorCode code, string? message, Exception? exception = null)
        {
            if (code == ErrorCode.None)
                throw new ArgumentException("AppError can't have code None.", nameof(code));
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("The message can't be empty.", nameof(message));

            Code = code;
            Message = message;
            Exception = exception;
        }


        //Factories

        public static AppError InvalidUrl(string url) => new(ErrorCode.InvalidUrl, $"The URL isn't valid: {url}");

        public static AppError EmptyPlaylist() => new(ErrorCode.EmptyPlaylist, "The playlist no contain any element.");

        public static AppError EmptyFolder(string path) => new(ErrorCode.EmptyFolder, $"The folder no contain any file: {path}");

        public static AppError NetworkFailure(string message, Exception? ex = null) => new(ErrorCode.NetworkFailure, message, ex);

        public static AppError PlaylistNotFound(string url) => new(ErrorCode.PlaylistNotFound, $"Playlist not found (404): {url}");

        public static AppError YoutubeApiError(string message, Exception? ex = null) => new(ErrorCode.YoutubeApiError, message, ex);

        public static AppError FileNotFound(string path) => new(ErrorCode.FileNotFound, $"File not found (404): {path}");

        public static AppError AccessDenied(string path, Exception? ex = null) => 
            new(ErrorCode.AccessDenied, $"Permission insuficience for: {path}", ex);

        public static AppError IoFailure(string message, Exception? ex = null) => new(ErrorCode.IoFailure, message, ex);

        public static AppError CollisionDetected(string path) => new(ErrorCode.CollisionDetected, $"Collision of names detected: {path}");

        public static AppError InvalidFileName(string name) => new(ErrorCode.InvalidFileName, $"The name contain invalid characters: {name}");

        public static AppError CancelledOperation() => new(ErrorCode.CancelledOperation, "Operation was cancelled.");

        public static AppError UnknownError(Exception ex) => new(ErrorCode.UnknownError, "Unknow Error.", ex);



        public bool Equals(AppError? other)
        {
            if (other is null) return false;
            if(ReferenceEquals(this, other)) return true;

            return Code == other.Code && Message == other.Message;
        }

        public override bool Equals(object? obj) => Equals(obj as AppError);

        public override int GetHashCode() => HashCode.Combine(Code, Message);

        public override string ToString() => $"[{Code}] {Message}";
        
    }
}
