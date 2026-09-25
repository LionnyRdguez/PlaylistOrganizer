using PlaylistOrganizer.Core.Errors;

namespace PlaylistOrganizer.Core.Results;

public static class ResultExtensions
{
 
    public static Result Tap(this Result result, Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (result.IsSuccess) action();
        return result;
    }

    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (result.IsSuccess) action(result.Value);
        return result;
    }

    public static Result<TOut> Map<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> mapper)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(mapper);

        return result.IsSuccess
            ? Result<TOut>.Success(mapper(result.Value))
            : Result<TOut>.Failure(result.Error!);
    }


    public static Result<TOut> Bind<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Result<TOut>> next)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(next);

        return result.IsSuccess
            ? next(result.Value)
            : Result<TOut>.Failure(result.Error!);
    }

    public static Result Bind<TIn>(
        this Result<TIn> result,
        Func<TIn, Result> next)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(next);

        return result.IsSuccess
            ? next(result.Value)
            : Result.Failure(result.Error!);
    }


    public static Result<T> MapError<T>(
        this Result<T> result,
        Func<AppError, AppError> mapper)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(mapper);

        return result.IsSuccess
            ? result
            : Result<T>.Failure(mapper(result.Error!));
    }


    public static Result Combine(params Result[] results)
    {
        ArgumentNullException.ThrowIfNull(results);

        foreach (var result in results)
        {
            if (result.IsFailure) return result;
        }

        return Result.Success();
    }
}