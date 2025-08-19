namespace Shouldly;

public class JsonComparisonResultTest
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResult()
    {
        var result = JsonComparisonResult.Success();

        result.IsEqual.ShouldBeTrue();
        result.FirstDifference.ShouldBeNull();
        result.ErrorMessage.ShouldBeNull();
    }

    [Fact]
    public void Failure_WithDifference_ShouldCreateFailedResult()
    {
        var difference = JsonDifference.ValueMismatch("/name", "John", "Jane");
        var result = JsonComparisonResult.Failure(difference);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldBe(difference);
        result.ErrorMessage.ShouldBe("JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void Failure_WithCustomMessage_ShouldCreateFailedResult()
    {
        var result = JsonComparisonResult.Failure("Custom error message");

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldBeNull();
        result.ErrorMessage.ShouldBe("Custom error message");
    }

    [Fact]
    public void GetErrorMessage_WhenSuccessful_ShouldReturnEmptyString()
    {
        var result = JsonComparisonResult.Success();

        result.GetErrorMessage().ShouldBe(string.Empty);
        result.GetErrorMessage("Custom message").ShouldBe(string.Empty);
    }

    [Fact]
    public void GetErrorMessage_WithoutCustomMessage_ShouldReturnErrorMessage()
    {
        var difference = JsonDifference.ValueMismatch("/name", "John", "Jane");
        var result = JsonComparisonResult.Failure(difference);

        result.GetErrorMessage().ShouldBe("JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void GetErrorMessage_WithCustomMessage_ShouldCombineMessages()
    {
        var difference = JsonDifference.ValueMismatch("/name", "John", "Jane");
        var result = JsonComparisonResult.Failure(difference);

        result.GetErrorMessage("JSON strings should be semantically the same")
            .ShouldBe("JSON strings should be semantically the same. JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void GetErrorMessage_WithCustomMessageOnly_ShouldReturnCustomMessage()
    {
        var result = JsonComparisonResult.Failure("Custom error");

        result.GetErrorMessage("User message").ShouldBe("User message. Custom error");
    }

    [Fact]
    public void GetErrorMessage_WithCustomMessageAndNoErrorMessage_ShouldReturnCustomMessage()
    {
        var result = new JsonComparisonResult { IsEqual = false };

        result.GetErrorMessage("User message").ShouldBe("User message. JSON comparison failed");
    }
}
