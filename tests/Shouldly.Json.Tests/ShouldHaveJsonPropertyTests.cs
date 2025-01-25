namespace Shouldly;

public class ShouldHaveJsonPropertyTests
{
    [Theory]
    [InlineData(@"{""prop"": ""value""}", "/prop")]
    [InlineData(@"{""prop"": null}", "/prop")]
    [InlineData(@"{""prop"": 42}", "/prop")]
    [InlineData(@"{""prop"": true}", "/prop")]
    [InlineData(@"{""prop"": []}", "/prop")]
    [InlineData(@"{""prop"": {}}", "/prop")]
    public void ShouldHaveJsonProperty_WithExistingProperty_ShouldNotThrow(string json, string pointer)
    {
        json.ShouldHaveJsonProperty(pointer);
    }

    [Theory]
    [InlineData(@"{""nested"": {""prop"": ""value""}}", "/nested/prop")]
    [InlineData(@"{""deeply"": {""nested"": {""prop"": null}}}", "/deeply/nested/prop")]
    [InlineData(@"{""array"": [{""prop"": ""value""}]}", "/array/0/prop")]
    public void ShouldHaveJsonProperty_WithNestedProperty_ShouldNotThrow(string json, string pointer)
    {
        json.ShouldHaveJsonProperty(pointer);
    }

    [Theory]
    [InlineData(@"{}", "/prop")]
    [InlineData(@"{""other"": ""value""}", "/prop")]
    [InlineData(@"{""nested"": {}}", "/nested/prop")]
    [InlineData(@"{""array"": []}", "/array/0")]
    public void ShouldHaveJsonProperty_WithMissingProperty_ShouldThrow(string json, string pointer)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonProperty(pointer))
            .Message.ShouldContain($"pointer '{pointer}'");
    }

    [Theory]
    [InlineData(@"{""prop"": ""value""}", "#/prop")]
    [InlineData(@"{""nested"": {""prop"": null}}", "#/nested/prop")]
    public void ShouldHaveJsonProperty_WithUriFragmentPointer_ShouldWork(string json, string pointer)
    {
        json.ShouldHaveJsonProperty(pointer);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldHaveJsonProperty_WithInvalidJson_ShouldThrow(string? json)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonProperty("/prop"));
    }

    [Fact]
    public void ShouldHaveJsonProperty_WithInvalidJsonSyntax_ShouldThrow()
    {
        var invalidJson = @"{""prop"": missing_quotes}";
        
        var ex = Should.Throw<ShouldAssertException>(() => invalidJson.ShouldHaveJsonProperty("/prop"));

        ex.Message.ShouldContain("Invalid JSON");
    }

    [Fact]
    public void ShouldHaveJsonProperty_WithInvalidPointer_ShouldThrow()
    {
        var json = @"{""prop"": ""value""}";
        var invalidPointer = "invalid pointer";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonProperty(invalidPointer));

        ex.Message.ShouldContain("Invalid JSON pointer");
    }

    [Fact]
    public void ShouldHaveJsonProperty_WithCustomMessage_ShouldIncludeMessage()
    {
        var json = @"{}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonProperty("/prop", customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
