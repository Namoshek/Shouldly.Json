namespace Shouldly;

using System;
using System.Collections.Generic;

public class ShouldHaveJsonValueAtTest
{
    [Fact]
    public void SimplePropertyAccess_ShouldWork()
    {
        var json = @"{""name"": ""John"", ""age"": 30}";

        json.ShouldHaveJsonValue("/name", "John");
        json.ShouldHaveJsonValue("/age", 30);
    }

    [Fact]
    public void NestedPropertyAccess_ShouldWork()
    {
        var json = @"{
            ""person"": {
                ""name"": ""John"",
                ""address"": {
                    ""city"": ""New York""
                }
            }
        }";

        json.ShouldHaveJsonValue("/person/name", "John");
        json.ShouldHaveJsonValue("/person/address/city", "New York");
    }

    [Fact]
    public void ArrayAccess_ShouldWork()
    {
        var json = @"{""numbers"": [1, 2, 3], ""names"": [""John"", ""Jane""]}";

        json.ShouldHaveJsonValue("/numbers/0", 1);
        json.ShouldHaveJsonValue("/names/1", "Jane");
    }

    [Fact]
    public void ComplexPath_ShouldWork()
    {
        var json = @"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""addresses"": [
                        {""city"": ""New York""},
                        {""city"": ""Boston""}
                    ]
                }
            ]
        }";

        json.ShouldHaveJsonValue("/users/0/name", "John");
        json.ShouldHaveJsonValue("/users/0/addresses/1/city", "Boston");
    }

    [Fact]
    public void CustomComparer_ShouldBeUsed()
    {
        var json = @"{""value"": ""TEST""}";

        json.ShouldHaveJsonValue("/value", "test", StringComparer.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("/invalid/path")]
    [InlineData("invalid")]
    [InlineData("~invalid")]
    public void InvalidPointer_ShouldThrow(string pointer)
    {
        var json = @"{""name"": ""John""}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValue(pointer, "value"));
    }

    [Fact]
    public void InvalidArrayIndex_ShouldThrow()
    {
        var json = @"{""array"": [1, 2, 3]}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValue("/array/5", 0));
    }

    [Fact]
    public void TypeMismatch_ShouldThrow()
    {
        var json = @"{""value"": ""string""}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValue<int>("/value", 42));
    }

    [Fact]
    public void NullValue_ShouldWork()
    {
        var json = @"{""value"": null}";

        json.ShouldHaveJsonValue<string?>("/value", null);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldHaveJsonValue("/path", "value"));
    }

    [Fact]
    public void CustomMessage_ShouldBeIncluded()
    {
        var json = @"{""value"": ""actual""}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldHaveJsonValue("/value", "expected", customMessage: customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void EscapedCharacters_ShouldWork()
    {
        var json = @"{""~tilde"": 1, ""slash/prop"": 2}";

        json.ShouldHaveJsonValue("/~0tilde", 1);
        json.ShouldHaveJsonValue("/slash~1prop", 2);
    }

    [Fact]
    public void ComplexType_ShouldWork()
    {
        var json = @"{""value"": {""name"": ""test""}}";
        var expected = "TeSt";

        json.ShouldHaveJsonValue("/value/name", expected, new CustomValueComparer());
    }
}

file class CustomValueComparer : IEqualityComparer<string>
{
    public bool Equals(string? x, string? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;

        return x.Equals(y, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(string value) => value.GetHashCode();
}
