using FluentAssertions;
using PlaylistOrganizer.Core.Errors;
using PlaylistOrganizer.Core.Results;

namespace PlaylistOrganizer.Tests.Results;


public class ResultExtensionsTests
{
    
    // Tap
    

    [Fact]
    public void Tap_OnSuccess_ShouldInvokeAction()
    {
        var result = Result.Success();
        var invoked = false;

        var returned = result.Tap(() => invoked = true);

        invoked.Should().BeTrue();
        returned.Should().BeSameAs(result);
    }

    [Fact]
    public void Tap_OnFailure_ShouldNotInvokeAction()
    {
        var result = Result.Failure(AppError.CancelledOperation());
        var invoked = false;

        result.Tap(() => invoked = true);

        invoked.Should().BeFalse();
    }

    [Fact]
    public void TapGeneric_OnSuccess_ShouldPassValueToAction()
    {
        var result = Result<int>.Success(42);
        var captured = 0;

        result.Tap(v => captured = v);

        captured.Should().Be(42);
    }

    [Fact]
    public void Tap_WithNullAction_ShouldThrow()
    {
        var act = () => Result.Success().Tap((Action)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    
    // Map
    

    [Fact]
    public void Map_OnSuccess_ShouldTransformValue()
    {
        var result = Result<int>.Success(42);

        var mapped = result.Map(v => v.ToString());

        mapped.IsSuccess.Should().BeTrue();
        mapped.Value.Should().Be("42");
    }

    [Fact]
    public void Map_OnFailure_ShouldPreserveError()
    {
        var error = AppError.InvalidUrl("http://x");
        var result = Result<int>.Failure(error);

        var mapped = result.Map(v => v.ToString());

        mapped.IsFailure.Should().BeTrue();
        mapped.Error.Should().BeSameAs(error);
    }

    
    // Bind
    

    [Fact]
    public void Bind_OnSuccess_ShouldChainResult()
    {
        var result = Result<int>.Success(42);

        var bound = result.Bind(v => Result<string>.Success($"value={v}"));

        bound.IsSuccess.Should().BeTrue();
        bound.Value.Should().Be("value=42");
    }

    [Fact]
    public void Bind_OnFailure_ShouldNotInvokeNext()
    {
        var result = Result<int>.Failure(AppError.CancelledOperation());
        var invoked = false;

        var bound = result.Bind(v => { invoked = true; return Result<string>.Success("x"); });

        invoked.Should().BeFalse();
        bound.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Bind_WithNonGenericNext_ShouldWork()
    {
        var result = Result<int>.Success(42);

        var bound = result.Bind(_ => Result.Success());

        bound.IsSuccess.Should().BeTrue();
    }

    
    // MapError
    

    [Fact]
    public void MapError_OnSuccess_ShouldReturnSameResult()
    {
        var result = Result<int>.Success(42);

        var mapped = result.MapError(_ => AppError.CancelledOperation());

        mapped.Should().BeSameAs(result);
    }

    [Fact]
    public void MapError_OnFailure_ShouldTransformError()
    {
        var original = AppError.InvalidUrl("http://x");
        var replacement = AppError.NetworkFailure("wrapped");

        var result = Result<int>.Failure(original);
        var mapped = result.MapError(_ => replacement);

        mapped.IsFailure.Should().BeTrue();
        mapped.Error.Should().BeSameAs(replacement);
    }

    
    // Combine
   

    [Fact]
    public void Combine_AllSuccesses_ShouldReturnSuccess()
    {
        var result = ResultExtensions.Combine(
            Result.Success(),
            Result.Success(),
            Result.Success());

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Combine_Empty_ShouldReturnSuccess()
    {
        var result = ResultExtensions.Combine();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Combine_WithOneFailure_ShouldReturnThatFailure()
    {
        var error = AppError.InvalidUrl("http://x");

        var result = ResultExtensions.Combine(
            Result.Success(),
            Result.Failure(error),
            Result.Success());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void Combine_WithMultipleFailures_ShouldReturnFirstFailure()
    {
        var first = AppError.InvalidUrl("http://first");
        var second = AppError.CancelledOperation();

        var result = ResultExtensions.Combine(
            Result.Failure(first),
            Result.Failure(second));

        result.Error.Should().BeSameAs(first);
    }
}