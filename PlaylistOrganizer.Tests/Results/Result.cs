using FluentAssertions;
using PlaylistOrganizer.Core.Errors;
using PlaylistOrganizer.Core.Results;

namespace PlaylistOrganizer.Tests.Results;


public class ResultTests
{
    
    // Factorías de creación
    

    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        var error = AppError.InvalidUrl("http://bad");

        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    [Fact]
    public void Failure_WithNullError_ShouldThrowArgumentNullException()
    {
        var act = () => Result.Failure(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("error");
    }

    
    // Match
    

    [Fact]
    public void Match_OnSuccess_ShouldInvokeOnSuccessBranch()
    {
        var result = Result.Success();

        var output = result.Match(
            onSuccess: () => "ok",
            onFailure: _ => "ko");

        output.Should().Be("ok");
    }

    [Fact]
    public void Match_OnFailure_ShouldInvokeOnFailureBranchWithError()
    {
        var error = AppError.CancelledOperation();
        var result = Result.Failure(error);

        AppError? capturedError = null;
        var output = result.Match(
            onSuccess: () => "ok",
            onFailure: e => { capturedError = e; return "ko"; });

        output.Should().Be("ko");
        capturedError.Should().BeSameAs(error);
    }

    [Fact]
    public void Match_WithNullCallbacks_ShouldThrow()
    {
        var result = Result.Success();

        var actSuccess = () => result.Match<int>(null!, _ => 0);
        var actFailure = () => result.Match(() => 0, (Func<AppError, int>)null!);

        actSuccess.Should().Throw<ArgumentNullException>();
        actFailure.Should().Throw<ArgumentNullException>();
    }

    
    // Igualdad por valor
   

    [Fact]
    public void Equals_TwoSuccesses_ShouldBeTrue()
    {
        var a = Result.Success();
        var b = Result.Success();

        a.Equals(b).Should().BeTrue();
        (a == b).Should().BeFalse(); // No hemos sobrecargado el operador, solo Equals.
    }

    [Fact]
    public void Equals_TwoFailuresWithSameError_ShouldBeTrue()
    {
        // Mismo código y mensaje => errores equivalentes (aunque sean instancias distintas).
        var error1 = AppError.InvalidUrl("http://x");
        var error2 = AppError.InvalidUrl("http://x");

        var a = Result.Failure(error1);
        var b = Result.Failure(error2);

        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoFailuresWithDifferentError_ShouldBeFalse()
    {
        var a = Result.Failure(AppError.InvalidUrl("http://x"));
        var b = Result.Failure(AppError.CancelledOperation());

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_SuccessAndFailure_ShouldBeFalse()
    {
        var success = Result.Success();
        var failure = Result.Failure(AppError.CancelledOperation());

        success.Equals(failure).Should().BeFalse();
        failure.Equals(success).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ShouldBeFalse()
    {
        var result = Result.Success();

        result.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ForEqualResults_ShouldBeEqual()
    {
        var a = Result.Failure(AppError.InvalidUrl("http://x"));
        var b = Result.Failure(AppError.InvalidUrl("http://x"));

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    
    // ToString
    

    [Fact]
    public void ToString_OnSuccess_ShouldReturnSuccess()
    {
        Result.Success().ToString().Should().Be("Success");
    }

    [Fact]
    public void ToString_OnFailure_ShouldIncludeErrorCodeAndMessage()
    {
        var error = AppError.InvalidUrl("http://x");
        var result = Result.Failure(error);

        var text = result.ToString();

        text.Should().Contain("Failure");
        text.Should().Contain("InvalidUrl");
    }
}