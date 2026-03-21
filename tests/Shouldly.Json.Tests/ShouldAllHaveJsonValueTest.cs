namespace Shouldly;

using System;
using System.Collections.Generic;

public class ShouldAllHaveJsonValueTest
{
    [Fact]
    public void AllElementsHaveSameStringValue_ShouldNotThrow()
    {
        var json = @"{""users"": [{""lastName"": ""Smith""}, {""lastName"": ""Smith""}]}";

        json.ShouldAllHaveJsonValue("/users", "/lastName", "Smith");
    }

    [Fact]
    public void AllElementsHaveSameIntValue_ShouldNotThrow()
    {
        var json = @"{""items"": [{""count"": 5}, {""count"": 5}]}";

        json.ShouldAllHaveJsonValue("/items", "/count", 5);
    }

    [Fact]
    public void AllElementsHaveSameBoolValue_ShouldNotThrow()
    {
        var json = @"{""items"": [{""active"": true}, {""active"": true}]}";

        json.ShouldAllHaveJsonValue("/items", "/active", true);
    }

    [Fact]
    public void SomeElementsDifferentValue_ShouldThrow()
    {
        var json = @"{""users"": [{""lastName"": ""Smith""}, {""lastName"": ""Jones""}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValue("/users", "/lastName", "Smith"));

        ex.Message.ShouldContain("index 1");
        ex.Message.ShouldContain("Jones");
    }

    [Fact]
    public void AllElementsDifferentValue_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}, {""name"": ""Jane""}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValue("/users", "/name", "Other"));

        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("index 1");
    }

    [Fact]
    public void EmptyArray_ShouldNotThrow()
    {
        var json = @"{""users"": []}";

        json.ShouldAllHaveJsonValue("/users", "/name", "Smith");
    }

    [Fact]
    public void MissingProperty_ShouldThrow()
    {
        var json = @"{""users"": [{""lastName"": ""Smith""}, {""age"": 30}]}";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValue("/users", "/lastName", "Smith"));

        ex.Message.ShouldContain("index 1");
        ex.Message.ShouldContain("missing");
    }

    [Fact]
    public void CustomComparer_ShouldBeUsed()
    {
        var json = @"{""users"": [{""name"": ""SMITH""}, {""name"": ""SMITH""}]}";

        json.ShouldAllHaveJsonValue("/users", "/name", "smith", StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullExpectedValue_ShouldWork()
    {
        var json = @"{""items"": [{""value"": null}, {""value"": null}]}";

        json.ShouldAllHaveJsonValue<string?>("/items", "/value", null);
    }

    [Fact]
    public void NestedArrayPointer_ShouldWork()
    {
        var json = @"{""data"": {""users"": [{""status"": ""active""}, {""status"": ""active""}]}}";

        json.ShouldAllHaveJsonValue("/data/users", "/status", "active");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldAllHaveJsonValue("/users", "/name", "Smith"));
    }

    [Fact]
    public void CustomMessage_ShouldBeIncluded()
    {
        var json = @"{""users"": [{""name"": ""John""}]}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValue("/users", "/name", "Other", customMessage: customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
