namespace Shouldly;

public class ShouldNotHaveJsonArrayCountTest
{
    [Fact]
    public void WhenArrayHasDifferentCount_ShouldNotThrow()
    {
        var json = @"{""numbers"": [1, 2, 3]}";
        
        json.ShouldNotHaveJsonArrayCount(0, "/numbers");
        json.ShouldNotHaveJsonArrayCount(1, "/numbers");
        json.ShouldNotHaveJsonArrayCount(2, "/numbers");
        json.ShouldNotHaveJsonArrayCount(4, "/numbers");
    }

    [Fact]
    public void WhenArrayHasExpectedCount_ShouldThrow()
    {
        var json = @"{""numbers"": [1, 2, 3]}";
        
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(3, "/numbers"));
    }

    [Fact]
    public void WithEmptyArray_WhenExpectingZero_ShouldThrow()
    {
        var json = @"{""numbers"": []}";

        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(0, "/numbers"));
    }

    [Fact]
    public void WithCustomJsonPointer_WhenArrayHasDifferentCount_ShouldNotThrow()
    {
        var json = @"{""data"": {""numbers"": [1, 2, 3]}}";
        
        json.ShouldNotHaveJsonArrayCount(2, "/data/numbers");
    }

    [Fact]
    public void WithCustomJsonPointer_WhenArrayHasExpectedCount_ShouldThrow()
    {
        var json = @"{""data"": {""numbers"": [1, 2, 3]}}";
        
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(3, "/data/numbers"));
    }

    [Fact]
    public void WhenJsonIsNull_ShouldThrow()
    {
        string? json = null;
        
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(0));
    }

    [Fact]
    public void WhenJsonIsInvalid_ShouldThrow()
    {
        var json = @"{""numbers"": [1, 2,]}";
        
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(3, "/numbers"));
    }

    [Fact]
    public void WhenPointerDoesNotExist_ShouldThrow()
    {
        var json = @"{""numbers"": [1, 2, 3]}";
        
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(3, "/nonexistent"));
    }

    [Fact]
    public void WhenPointerIsInvalid_ShouldThrow()
    {
        var json = @"{""numbers"": [1, 2, 3]}";
        
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(3, "invalid"));
    }

    [Fact]
    public void WhenValueIsNotArray_ShouldThrow()
    {
        var json = @"{""value"": 42}";
        
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(1, "/value"));
    }

    [Fact]
    public void WithNestedArrays_ShouldCheckSpecifiedArray()
    {
        var json = @"{""matrix"": [[1, 2], [3, 4, 5]]}";
        
        json.ShouldNotHaveJsonArrayCount(3, "/matrix");
        json.ShouldNotHaveJsonArrayCount(3, "/matrix/0");

        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(2, "/matrix"));
    }

    [Fact]
    public void WithCustomMessage_ShouldUseProvidedMessage()
    {
        var json = @"{""numbers"": [1, 2, 3]}";
        var customMessage = "Array should not have exactly three elements!";
        
        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayCount(3, "/numbers", customMessage: customMessage));
        
        ex.Message.ShouldContain(customMessage);
    }
} 