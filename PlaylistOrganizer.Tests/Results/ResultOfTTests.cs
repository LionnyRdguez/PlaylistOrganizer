using FluentAssertions;
using PlaylistOrganizer.Core.Errors;
using PlaylistOrganizer.Core.Results;

namespace PlaylistOrganizer.Tests.Results;


public class ResultOfTTests
{
   
    // Factorías de creación
   

    [Fact]
    public void Success_WithValue_ShouldCreateSuccessfulResult()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_WithNullReference_ShouldThrow()
    {
        var act = () => Result<string>.Success(null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("value");
    }

    [Fact]
    public void Success_WithValueType_ShouldAllowDefaultValue()
    {
        // El valor por defecto de un tipo valor (0) no es null, así que es válido.
        var result = Result<int>.Success(0);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(0);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        var error = AppError.NetworkFailure("no connection");

        var result = Result<string>.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().BeSameAs(error);
    }

    
    // Acceso al valor
   

    [Fact]
    public void Value_OnFailure_ShouldThrowInvalidOperationException()
    {
        var result = Result<string>.Failure(AppError.CancelledOperation());

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    
    // TryGetValue
    

    [Fact]
    public void TryGetValue_OnSuccess_ShouldReturnTrueAndValue()
    {
        var result = Result<int>.Success(7);

        var ok = result.TryGetValue(out var value);

        ok.Should().BeTrue();
        value.Should().Be(7);
    }

    [Fact]
    public void TryGetValue_OnFailure_ShouldReturnFalseAndDefaultValue()
    {
        var result = Result<int>.Failure(AppError.CancelledOperation());

        var ok = result.TryGetValue(out var value);

        ok.Should().BeFalse();
        value.Should().Be(default(int));
    }

    
    // ToResult
    

    [Fact]
    public void ToResult_OnSuccess_ShouldReturnNonGenericSuccess()
    {
        var generic = Result<int>.Success(42);

        var nonGeneric = generic.ToResult();

        nonGeneric.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ToResult_OnFailure_ShouldPreserveError()
    {
        var error = AppError.InvalidUrl("http://x");
        var generic = Result<int>.Failure(error);

        var nonGeneric = generic.ToResult();

        nonGeneric.IsFailure.Should().BeTrue();
        nonGeneric.Error.Should().BeSameAs(error);
    }

    
    // Conversión implícita
    

    [Fact]
    public void ImplicitConversion_FromValue_ShouldCreateSuccess()
    {
        Result<int> result = 42;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    
    // Igualdad por valor
    

    [Fact]
    public void Equals_TwoSuccessesWithSameValue_ShouldBeTrue()
    {
        var a = Result<int>.Success(42);
        var b = Result<int>.Success(42);

        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void Equals_TwoSuccessesWithDifferentValue_ShouldBeFalse()
    {
        var a = Result<int>.Success(42);
        var b = Result<int>.Success(43);

        a.Equals(b).Should().BeFalse();
    }

    [Fact]
    public void Equals_TwoFailuresWithSameError_ShouldBeTrue()
    {
        var a = Result<int>.Failure(AppError.InvalidUrl("http://x"));
        var b = Result<int>.Failure(AppError.InvalidUrl("http://x"));

        a.Equals(b).Should().BeTrue();
    }

    [Fact]
    public void Equals_SuccessAndFailure_ShouldBeFalse()
    {
        var success = Result<int>.Success(42);
        var failure = Result<int>.Failure(AppError.CancelledOperation());

        success.Equals(failure).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_ForEqualResults_ShouldBeEqual()
    {
        var a = Result<int>.Success(42);
        var b = Result<int>.Success(42);

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    
    // Match
    

    [Fact]
    public void Match_OnSuccess_ShouldReceiveValue()
    {
        var result = Result<int>.Success(42);

        var output = result.Match(
            onSuccess: v => v * 2,
            onFailure: _ => -1);

        output.Should().Be(84);
    }

    [Fact]
    public void Match_OnFailure_ShouldReceiveError()
    {
        var error = AppError.CancelledOperation();
        var result = Result<int>.Failure(error);

        AppError? capturedError = null;
        var output = result.Match(
            onSuccess: _ => 0,
            onFailure: e => { capturedError = e; return -1; });

        output.Should().Be(-1);
        capturedError.Should().BeSameAs(error);
    }

    
    // ToString
    

    [Fact]
    public void ToString_OnSuccess_ShouldIncludeValue()
    {
        Result<int>.Success(42).ToString().Should().Contain("42").And.Contain("Success");
    }

    [Fact]
    public void ToString_OnFailure_ShouldIncludeError()
    {
        var text = Result<int>.Failure(AppError.InvalidUrl("http://x")).ToString();

        text.Should().Contain("Failure").And.Contain("InvalidUrl");
    }
}