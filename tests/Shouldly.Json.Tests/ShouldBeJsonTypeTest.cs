namespace Shouldly;

public class ShouldBeJsonTypeTest
{
    [Theory]
    [InlineData(@"{}")]
    [InlineData(@"{""prop"": ""value""}")]
    [InlineData(@"{""nested"": {""prop"": ""value""}}")]
    [InlineData(@"{""array"": [1, 2, 3]}")]
    public void ShouldBeJsonObject_WithValidObjects_ShouldNotThrow(string json)
    {
        json.ShouldBeJsonObject();
    }

    [Theory]
    [InlineData(@"null")]
    [InlineData(@"true")]
    [InlineData(@"42")]
    [InlineData(@"[]")]
    [InlineData(@"""string""")]
    public void ShouldBeJsonObject_WithNonObjects_ShouldThrow(string json)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonObject());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldBeJsonObject_WithInvalidInput_ShouldThrow(string? json)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonObject());
    }

    [Fact]
    public void ShouldBeJsonObject_WithInvalidJson_ShouldThrowDescriptiveError()
    {
        var invalidJson = @"{""prop"": missing_quotes}";
        
        var ex = Should.Throw<ShouldAssertException>(() => invalidJson.ShouldBeJsonObject());

        ex.Message.ShouldContain("JSON string should have an object as root element");
    }

    [Fact]
    public void ShouldBeJsonObject_WithCustomMessage_ShouldIncludeMessage()
    {
        var json = @"[]";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonObject(customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Theory]
    [InlineData(@"[]")]
    [InlineData(@"[1, 2, 3]")]
    [InlineData(@"[{""prop"": ""value""}]")]
    [InlineData(@"[[1, 2], [3, 4]]")]
    public void ShouldBeJsonArray_WithValidArrays_ShouldNotThrow(string json)
    {
        json.ShouldBeJsonArray();
    }

    [Theory]
    [InlineData(@"null")]
    [InlineData(@"true")]
    [InlineData(@"42")]
    [InlineData(@"{}")]
    [InlineData(@"""string""")]
    public void ShouldBeJsonArray_WithNonArrays_ShouldThrow(string json)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonArray());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldBeJsonArray_WithInvalidInput_ShouldThrow(string? json)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonArray());
    }

    [Fact]
    public void ShouldBeJsonArray_WithInvalidJson_ShouldThrowDescriptiveError()
    {
        var invalidJson = @"[1, 2, ]"; // trailing comma
        
        var ex = Should.Throw<ShouldAssertException>(() => invalidJson.ShouldBeJsonArray());

        ex.Message.ShouldContain("JSON string should have an array as root element");
    }

    [Fact]
    public void ShouldBeJsonArray_WithCustomMessage_ShouldIncludeMessage()
    {
        var json = @"{}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonArray(customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
