namespace Shouldly;

public class ShouldAllHaveJsonValueMatchingRegexTest
{
    // ShouldAllHaveJsonValueMatchingRegex

    [Fact]
    public void AllValuesMatchRegex_ShouldNotThrow()
    {
        var json = @"{""users"": [{""email"": ""john@example.com""}, {""email"": ""jane@test.org""}]}";

        json.ShouldAllHaveJsonValueMatchingRegex("/users", "/email",
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    }

    [Fact]
    public void AllNamesMatchCapitalized_ShouldNotThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}, {""name"": ""Jane""}]}";

        json.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z][a-z]+$");
    }

    [Fact]
    public void SomeValuesDoNotMatch_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}, {""name"": ""jane""}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z][a-z]+$"));

        ex.Message.ShouldContain("index 1");
        ex.Message.ShouldContain("jane");
    }

    [Fact]
    public void AllValuesDoNotMatch_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""john""}, {""name"": ""jane""}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z][a-z]+$"));

        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("index 1");
    }

    [Fact]
    public void EmptyArray_ShouldNotThrow()
    {
        var json = @"{""users"": []}";

        json.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z][a-z]+$");
    }

    [Fact]
    public void MissingProperty_ShouldThrow()
    {
        var json = @"{""users"": [{""age"": 30}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"^.*$"));

        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("missing");
    }

    [Fact]
    public void NullValue_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": null}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"^.*$"));

        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("null");
    }

    [Fact]
    public void InvalidRegex_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"["));

        ex.Message.ShouldContain("Invalid regex pattern");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() =>
            invalidJson.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"^.*$"));
    }

    [Fact]
    public void CustomMessage_ShouldBeIncluded()
    {
        var json = @"{""users"": [{""name"": ""john""}]}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z].*$", customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    // ShouldAllNotHaveJsonValueMatchingRegex

    [Fact]
    public void AllValuesDoNotMatchRegex_ShouldNotThrow()
    {
        var json = @"{""users"": [{""name"": ""john""}, {""name"": ""jane""}]}";

        json.ShouldAllNotHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z][a-z]+$");
    }

    [Fact]
    public void SomeValuesMatch_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}, {""name"": ""jane""}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllNotHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z][a-z]+$"));

        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("John");
    }

    [Fact]
    public void AllValuesMatch_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}, {""name"": ""Jane""}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllNotHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z][a-z]+$"));

        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("index 1");
    }

    [Fact]
    public void EmptyArray_NotMatchingRegex_ShouldNotThrow()
    {
        var json = @"{""users"": []}";

        json.ShouldAllNotHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z][a-z]+$");
    }

    [Fact]
    public void MissingProperty_NotMatchingRegex_ShouldThrow()
    {
        var json = @"{""users"": [{""age"": 30}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllNotHaveJsonValueMatchingRegex("/users", "/name", @"^.*$"));

        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("missing");
    }

    [Fact]
    public void InvalidRegex_NotMatching_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllNotHaveJsonValueMatchingRegex("/users", "/name", @"["));

        ex.Message.ShouldContain("Invalid regex pattern");
    }

    [Fact]
    public void CustomMessage_NotMatchingRegex_ShouldBeIncluded()
    {
        var json = @"{""users"": [{""name"": ""John""}]}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllNotHaveJsonValueMatchingRegex("/users", "/name", @"^[A-Z].*$", customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
