namespace Shouldly.Json.Tests;

public class ShouldNotHaveJsonPropertyTest
{
    [Theory]
    [InlineData(@"{}", "/prop")]
    [InlineData(@"{""other"": ""value""}", "/prop")]
    [InlineData(@"{""nested"": {}}", "/nested/prop")]
    [InlineData(@"{""array"": []}", "/array/0")]
    public void ShouldNotHaveJsonProperty_WithMissingProperty_ShouldNotThrow(string json, string pointer)
    {
        json.ShouldNotHaveJsonProperty(pointer);
    }

    [Theory]
    [InlineData(@"{""prop"": ""value""}", "/prop")]
    [InlineData(@"{""prop"": null}", "/prop")]
    [InlineData(@"{""prop"": 42}", "/prop")]
    [InlineData(@"{""prop"": true}", "/prop")]
    [InlineData(@"{""prop"": []}", "/prop")]
    [InlineData(@"{""prop"": {}}", "/prop")]
    public void ShouldNotHaveJsonProperty_WithExistingProperty_ShouldThrow(string json, string pointer)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonProperty(pointer))
            .Message.ShouldContain($"pointer '{pointer}'");
    }

    [Theory]
    [InlineData(@"{""nested"": {""prop"": ""value""}}", "/nested/prop")]
    [InlineData(@"{""deeply"": {""nested"": {""prop"": null}}}", "/deeply/nested/prop")]
    [InlineData(@"{""array"": [{""prop"": ""value""}]}", "/array/0/prop")]
    public void ShouldNotHaveJsonProperty_WithNestedProperty_ShouldThrow(string json, string pointer)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonProperty(pointer));
    }

    [Theory]
    [InlineData(@"{""prop"": ""value""}", "#/prop")]
    [InlineData(@"{""nested"": {""prop"": null}}", "#/nested/prop")]
    public void ShouldNotHaveJsonProperty_WithUriFragmentPointer_ShouldThrow(string json, string pointer)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonProperty(pointer));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldNotHaveJsonProperty_WithInvalidJson_ShouldThrow(string? json)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonProperty("/prop"));
    }

    [Fact]
    public void ShouldNotHaveJsonProperty_WithInvalidJsonSyntax_ShouldThrow()
    {
        var invalidJson = @"{""prop"": missing_quotes}";
        
        var ex = Should.Throw<ShouldAssertException>(() => invalidJson.ShouldNotHaveJsonProperty("/prop"));

        ex.Message.ShouldContain("Invalid JSON");
    }

    [Fact]
    public void ShouldNotHaveJsonProperty_WithInvalidPointer_ShouldThrow()
    {
        var json = @"{""prop"": ""value""}";
        var invalidPointer = "invalid pointer";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonProperty(invalidPointer));

        ex.Message.ShouldContain("Invalid JSON pointer");
    }

    [Fact]
    public void ShouldNotHaveJsonProperty_WithCustomMessage_ShouldIncludeMessage()
    {
        var json = @"{""prop"": ""value""}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonProperty("/prop", customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
