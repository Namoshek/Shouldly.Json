namespace Shouldly;

public class ShouldHaveJsonArrayCountTest
{
    [Theory]
    [InlineData(@"[]", 0)]
    [InlineData(@"[1]", 1)]
    [InlineData(@"[1, 2, 3]", 3)]
    public void ShouldHaveJsonArrayCount_WithRootArray_ShouldNotThrow(string json, int count)
    {
        json.ShouldHaveJsonArrayCount(count);
    }

    [Theory]
    [InlineData(@"{""array"": []}", 0, "/array")]
    [InlineData(@"{""array"": [1]}", 1, "/array")]
    [InlineData(@"{""array"": [1, 2, 3]}", 3, "/array")]
    public void ShouldHaveJsonArrayCount_WithNestedArray_ShouldNotThrow(string json, int count, string pointer)
    {
        json.ShouldHaveJsonArrayCount(count, pointer);
    }

    [Theory]
    [InlineData(@"{""nested"": {""array"": [1, 2]}}", 2, "/nested/array")]
    [InlineData(@"{""deeply"": {""nested"": {""array"": [1]}}}", 1, "/deeply/nested/array")]
    [InlineData(@"{""arrays"": [[1, 2], [3, 4]]}", 2, "/arrays/1")]
    public void ShouldHaveJsonArrayCount_WithDeeplyNestedArrays_ShouldNotThrow(string json, int count, string pointer)
    {
        json.ShouldHaveJsonArrayCount(count, pointer);
    }

    [Theory]
    [InlineData(@"[]", 1, "")]
    [InlineData(@"[1]", 2, "")]
    [InlineData(@"{""array"": [1, 2]}", 1, "/array")]
    public void ShouldHaveJsonArrayCount_WithIncorrectCount_ShouldThrow(string json, int expectedCount, string pointer)
    {
        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayCount(expectedCount, pointer));

        ex.Message.ShouldContain($"should have {expectedCount} elements");
    }

    [Theory]
    [InlineData(@"{}", "/array")]
    [InlineData(@"{""other"": []}", "/array")]
    [InlineData(@"{""nested"": {}}", "/nested/array")]
    public void ShouldHaveJsonArrayCount_WithMissingArray_ShouldThrow(string json, string pointer)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayCount(1, pointer))
            .Message.ShouldContain($"No value found at JSON pointer '{pointer}'");
    }

    [Theory]
    [InlineData(@"{""value"": 42}", "/value")]
    [InlineData(@"{""value"": ""string""}", "/value")]
    [InlineData(@"{""value"": true}", "/value")]
    [InlineData(@"{""value"": null}", "/value")]
    [InlineData(@"{""value"": {}}", "/value")]
    public void ShouldHaveJsonArrayCount_WithNonArrayValue_ShouldThrow(string json, string pointer)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayCount(1, pointer))
            .Message.ShouldContain("is not an array");
    }

    [Theory]
    [InlineData(@"[1, 2, 3]", 3, "")]
    [InlineData(@"{""array"": [1, 2, 3]}", 3, "#/array")]
    public void ShouldHaveJsonArrayCount_WithUriFragmentPointer_ShouldWork(string json, int count, string pointer)
    {
        json.ShouldHaveJsonArrayCount(count, pointer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldHaveJsonArrayCount_WithInvalidJson_ShouldThrow(string? json)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayCount(1));
    }

    [Fact]
    public void ShouldHaveJsonArrayCount_WithInvalidJsonSyntax_ShouldThrow()
    {
        var invalidJson = @"{""array"": [1, 2,]}"; // trailing comma
        
        var ex = Should.Throw<ShouldAssertException>(() => invalidJson.ShouldHaveJsonArrayCount(2, "/array"));

        ex.Message.ShouldContain("Invalid JSON");
    }

    [Fact]
    public void ShouldHaveJsonArrayCount_WithInvalidPointer_ShouldThrow()
    {
        var json = @"[1, 2, 3]";
        var invalidPointer = "invalid pointer";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayCount(3, invalidPointer));

        ex.Message.ShouldContain("Invalid JSON pointer");
    }

    [Fact]
    public void ShouldHaveJsonArrayCount_WithCustomMessage_ShouldIncludeMessage()
    {
        var json = @"[1, 2]";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayCount(3, customMessage: customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
