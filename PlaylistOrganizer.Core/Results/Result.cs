using PlaylistOrganizer.Core.Errors;

namespace PlaylistOrganizer.Core.Results
{
    public class Result : IEquatable<Result>
    {
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public AppError? Error { get; }

        protected Result(bool isSuccess, AppError? error)
        {
            if (isSuccess && error is not null)
                throw new InvalidOperationException("A successful result cannot have an error.");
            if (!isSuccess && error is null)
                throw new InvalidOperationException("A failure result must have an error.");
            IsSuccess = isSuccess;
            Error = error;
        }

        //Factories 

        public static Result Success() => new(true, null); // Crea un resultado exitoso sin valor de error.

        public static Result Failure(AppError error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new(false, error);
        }

        public TResult Match<TResult>(Func<TResult> onSuccess, Func<AppError, TResult> onFailure)
        {
            ArgumentNullException.ThrowIfNull(onSuccess);
            ArgumentNullException.ThrowIfNull(onFailure);
            return IsSuccess ? onSuccess() : onFailure(Error!);
        }

        public bool Equals(Result? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (IsSuccess != other.IsSuccess) return false;

            return IsSuccess || Error!.Equals(other.Error!);
        }

        public override bool Equals(object? obj) => Equals(obj as Result);

        public override int GetHashCode() => IsSuccess ? HashCode.Combine(true) : HashCode.Combine(false, Error);

        public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";

    }

    public sealed class Result<T> : Result, IEquatable<Result<T>>
    {
        private readonly T? _value;

        public T Value => IsSuccess ? _value! : throw new InvalidOperationException("Cannot access the value of a failure result.");

        private Result(bool isSuccess, T? value, AppError? error) : base(isSuccess, error)
        {

            if (isSuccess && value is null && default(T) is null)
            {
                throw new InvalidOperationException("A successful result must carry a non-null value.");
            }
            _value = value;
        }

        public static Result<T> Success(T value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value), "Cannot create a successful result with a null value.");

            }

            return new(true, value, null);
        }

        public static new Result<T> Failure(AppError error)
        {
            ArgumentNullException.ThrowIfNull(error);
            return new(false, default, error);
        }

        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<AppError, TResult> onFailure)
        {
            ArgumentNullException.ThrowIfNull(onSuccess);
            ArgumentNullException.ThrowIfNull(onFailure);

            return IsSuccess ? onSuccess(_value!) : onFailure(Error!);
        }

        public bool TryGetValue(out T value)
        {
            if (IsSuccess)
            {
                value = _value!;
                return true;
            }

            value = default!;
            return false;
        }

        public Result ToResult() => IsSuccess ? Result.Success() : Result.Failure(Error!);

        public static implicit operator Result<T>(T value) => Success(value);

        public bool Equals(Result<T>? other)
        {

            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (IsSuccess != other.IsSuccess) return false;

            return IsSuccess ? EqualityComparer<T>.Default.Equals(_value!, other._value!) : Error!.Equals(other.Error!);


        }

        public override bool Equals(object? obj) => Equals(obj as Result<T>);

        public override int GetHashCode() => IsSuccess ? HashCode.Combine(true,_value) : HashCode.Combine(false,Error);

        public override string ToString() => IsSuccess ? $"Success: {_value}" : $"Failure: {Error}";
        
        

    }
}
