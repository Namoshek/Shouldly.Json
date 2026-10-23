using System;

namespace Shouldly;

public class ShouldAnyHaveJsonValueTest
{
    private const string ItemsJson = @"{""items"": [{""id"": 1, ""status"": ""active""}, {""id"": 2, ""status"": ""inactive""}, {""id"": 3, ""status"": ""pending""}]}";

    [Fact]
    public void ShouldAnyHaveJsonProperty_WhenAnyElementHasProperty_ShouldNotThrow()
    {
        ItemsJson.ShouldAnyHaveJsonProperty("/items", "/status");
    }

    [Fact]
    public void ShouldAnyHaveJsonProperty_WhenNoElementHasProperty_ShouldThrow()
    {
        var json = @"{""items"": [{""id"": 1}, {""id"": 2}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAnyHaveJsonProperty("/items", "/status"));

        ex.Message.ShouldContain("should have a property at pointer '/status'");
        ex.Message.ShouldContain("(found at no index)");
    }

    [Fact]
    public void ShouldNotAnyHaveJsonPropertyReportsMatchingIndices()
    {
        var json = @"{""items"": [{""id"": 1}, {""status"": ""ok""}, {""id"": 3}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotAnyHaveJsonProperty("/items", "/id"));

        ex.Message.ShouldContain("found at indices: 0, 2");
    }

    [Fact]
    public void ShouldAnyHaveJsonProperty_WithArrayAsRoot_ShouldWork()
    {
        @"[{""a"": 1}, {""b"": 2}]".ShouldAnyHaveJsonProperty("", "/a");
    }

    [Fact]
    public void ShouldAnyHaveJsonProperty_NonArrayPointer_ShouldThrow()
    {
        var json = @"{""items"": {""a"": 1}}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAnyHaveJsonProperty("/items", "/a"));

        ex.Message.ShouldContain("is not an array");
    }

    [Fact]
    public void ShouldAnyHaveJsonProperty_InvalidPointer_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ItemsJson.ShouldAnyHaveJsonProperty("/items", "bad~pointer"));

        ex.Message.ShouldContain("Invalid JSON pointer");
    }

    [Fact]
    public void ShouldAnyHaveJsonProperty_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => @"{""items"": [{""id"": 1}]}".ShouldAnyHaveJsonProperty("/items", "/status", customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void ShouldNotAnyHaveJsonProperty_WhenNoElementHasProperty_ShouldNotThrow()
    {
        @"{""items"": [{""id"": 1}, {""id"": 2}]}".ShouldNotAnyHaveJsonProperty("/items", "/status");
    }

    [Fact]
    public void ShouldNotAnyHaveJsonProperty_WhenAnyElementHasProperty_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ItemsJson.ShouldNotAnyHaveJsonProperty("/items", "/status"));

        ex.Message.ShouldContain("should have a property at pointer '/status'");
        ex.Message.ShouldContain("found at indices: 0, 1, 2");
    }

    [Fact]
    public void ShouldNotAnyHaveJsonProperty_WithNullProperty_ShouldThrow()
    {
        var json = @"{""items"": [{""status"": null}]}";

        Should.Throw<ShouldAssertException>(() => json.ShouldNotAnyHaveJsonProperty("/items", "/status"));
    }

    // ShouldAnyHaveJsonValue

    [Fact]
    public void ShouldAnyHaveJsonValue_WhenAnyElementMatches_ShouldNotThrow()
    {
        ItemsJson.ShouldAnyHaveJsonValue("/items", "/id", 2);
    }

    [Fact]
    public void ShouldAnyHaveJsonValue_WhenNoElementMatches_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ItemsJson.ShouldAnyHaveJsonValue("/items", "/id", 99));

        ex.Message.ShouldContain("At least one element in JSON array at pointer '/items' should have value '99' at property '/id'");
        ex.Message.ShouldContain("observed values at index 0: '1'");
    }

    [Fact]
    public void ShouldAnyHaveJsonValueStringValues_ShouldWork()
    {
        ItemsJson.ShouldAnyHaveJsonValue("/items", "/status", "inactive");
    }

    [Fact]
    public void ShouldAnyHaveJsonValue_WithComparer_ShouldUseComparer()
    {
        var json = @"{""items"": [{""name"": ""Alice""}, {""name"": ""Bob""}]}";

        json.ShouldAnyHaveJsonValue("/items", "/name", "alice", StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void ShouldAnyHaveJsonValue_WithNullExpectedValue_ShouldThrowWhenMissing()
    {
        var json = @"{""items"": [{""id"": 1}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAnyHaveJsonValue<string?>("/items", "/missing", null));

        ex.Message.ShouldContain("should have value");
    }

    [Fact]
    public void ShouldAnyHaveJsonValue_TypeMismatch_ShouldThrow()
    {
        var json = @"{""items"": [{""id"": ""text""}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAnyHaveJsonValue("/items", "/id", 42));

        ex.Message.ShouldContain("observed values at index 0");
        ex.Message.ShouldContain("(type mismatch)");
    }

    [Fact]
    public void ShouldAnyHaveJsonValue_NonArrayPointer_ShouldThrow()
    {
        var json = @"{""items"": ""not an array""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAnyHaveJsonValue("/items", "/id", 1));

        ex.Message.ShouldContain("is not an array");
    }

    [Fact]
    public void ShouldAnyHaveJsonValue_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => ItemsJson.ShouldAnyHaveJsonValue("/items", "/id", 99, null, customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    // ShouldAllNotHaveJsonValue

    [Fact]
    public void ShouldAllNotHaveJsonValue_WhenNoElementMatches_ShouldNotThrow()
    {
        ItemsJson.ShouldAllNotHaveJsonValue("/items", "/id", 99);
    }

    [Fact]
    public void ShouldAllNotHaveJsonValue_WhenAnyElementMatches_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ItemsJson.ShouldAllNotHaveJsonValue("/items", "/status", "pending"));

        ex.Message.ShouldContain("No element in JSON array at pointer '/items' should have value 'pending' at property '/status'");
        ex.Message.ShouldContain("matching values at index 2: 'pending'");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValue_WhenAllElementsMatch_ShouldThrow()
    {
        var json = @"{""items"": [{""id"": 1}, {""id"": 1}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllNotHaveJsonValue("/items", "/id", 1));

        ex.Message.ShouldContain("matching values at index 0: '1', index 1: '1'");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValue_WithNullValues_ShouldThrow()
    {
        var json = @"{""items"": [{""status"": null}, {""status"": ""active""}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllNotHaveJsonValue<string?>("/items", "/status", null));

        ex.Message.ShouldContain("matching values at index 0");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValue_MissingProperty_ShouldNotThrow()
    {
        var json = @"{""items"": [{""id"": 1}, {""id"": 2}]}";

        json.ShouldAllNotHaveJsonValue<string?>("/items", "/status", "unexpected");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValue_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => ItemsJson.ShouldAllNotHaveJsonValue("/items", "/status", "active", null, customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    // ShouldAnyHaveJsonValueMatchingRegex / ShouldAnyNotHaveJsonValueMatchingRegex

    [Fact]
    public void ShouldAnyHaveJsonValueMatchingRegex_WhenAnyMatches_ShouldNotThrow()
    {
        @"{""items"": [{""email"": ""a@b.com""}, {""email"": ""invalid""}]}".ShouldAnyHaveJsonValueMatchingRegex("/items", "/email", @"^[^@]+@[^@]+\.[^@]+$");
    }

    [Fact]
    public void ShouldAnyHaveJsonValueMatchingRegex_WhenNoneMatches_ShouldThrow()
    {
        var json = @"{""items"": [{""email"": ""invalid""}, {""email"": ""also invalid""}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAnyHaveJsonValueMatchingRegex("/items", "/email", @"^[^@]+@[^@]+\.[^@]+$"));

        ex.Message.ShouldContain("should have value at '/email' matching regex pattern");
        ex.Message.ShouldContain("match regex");
        ex.Message.ShouldContain("observed values at index 0");
    }

    [Fact]
    public void ShouldAnyNotHaveJsonValueMatchingRegex_WhenNoneMatches_ShouldNotThrow()
    {
        @"{""items"": [{""code"": ""abc""}, {""code"": ""def""}]}".ShouldAnyNotHaveJsonValueMatchingRegex("/items", "/code", @"^\d+$");
    }

    [Fact]
    public void ShouldAnyNotHaveJsonValueMatchingRegex_WhenAnyMatches_ShouldThrow()
    {
        var json = @"{""items"": [{""code"": ""abc""}, {""code"": ""123""}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAnyNotHaveJsonValueMatchingRegex("/items", "/code", @"^\d+$"));

        ex.Message.ShouldContain("No element in JSON array at pointer '/items'");
        ex.Message.ShouldContain("not match regex");
        ex.Message.ShouldContain("matching values at index 1: '123'");
    }

    [Fact]
    public void AnyRegex_NullValueAtSomeIndex_ShouldNotCountAsMatch()
    {
        @"{""items"": [{""code"": null}, {""code"": ""abc""}]}".ShouldAnyNotHaveJsonValueMatchingRegex("/items", "/code", @"^\d+$");
    }

    [Fact]
    public void AnyRegex_InvalidRegex_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ItemsJson.ShouldAnyHaveJsonValueMatchingRegex("/items", "/status", "[invalid("));

        ex.Message.ShouldContain("Invalid regex pattern");
    }

    [Fact]
    public void AnyRegex_NonArrayPointer_ShouldThrow()
    {
        var json = @"{""items"": 1}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAnyHaveJsonValueMatchingRegex("/items", "/status", "."));

        ex.Message.ShouldContain("is not an array");
    }

    [Fact]
    public void AnyRegex_InvalidJson_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "{invalid json}".ShouldAnyHaveJsonValueMatchingRegex("/items", "/status", "."));

        ex.Message.ShouldContain("Invalid JSON");
    }
}
