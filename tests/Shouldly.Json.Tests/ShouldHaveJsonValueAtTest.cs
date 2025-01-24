namespace Shouldly;

using System;
using System.Collections.Generic;

public class ShouldHaveJsonValueAtTest
{
    [Fact]
    public void SimplePropertyAccess_ShouldWork()
    {
        var json = @"{""name"": ""John"", ""age"": 30}";

        json.ShouldHaveJsonValueAt("/name", "John");
        json.ShouldHaveJsonValueAt("/age", 30);
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

        json.ShouldHaveJsonValueAt("/person/name", "John");
        json.ShouldHaveJsonValueAt("/person/address/city", "New York");
    }

    [Fact]
    public void ArrayAccess_ShouldWork()
    {
        var json = @"{""numbers"": [1, 2, 3], ""names"": [""John"", ""Jane""]}";

        json.ShouldHaveJsonValueAt("/numbers/0", 1);
        json.ShouldHaveJsonValueAt("/names/1", "Jane");
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

        json.ShouldHaveJsonValueAt("/users/0/name", "John");
        json.ShouldHaveJsonValueAt("/users/0/addresses/1/city", "Boston");
    }

    [Fact]
    public void CustomComparer_ShouldBeUsed()
    {
        var json = @"{""value"": ""TEST""}";

        json.ShouldHaveJsonValueAt("/value", "test", StringComparer.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("/invalid/path")]
    [InlineData("invalid")]
    [InlineData("~invalid")]
    public void InvalidPointer_ShouldThrow(string pointer)
    {
        var json = @"{""name"": ""John""}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueAt(pointer, "value"));
    }

    [Fact]
    public void InvalidArrayIndex_ShouldThrow()
    {
        var json = @"{""array"": [1, 2, 3]}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueAt("/array/5", 0));
    }

    [Fact]
    public void TypeMismatch_ShouldThrow()
    {
        var json = @"{""value"": ""string""}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueAt<int>("/value", 42));
    }

    [Fact]
    public void NullValue_ShouldWork()
    {
        var json = @"{""value"": null}";

        json.ShouldHaveJsonValueAt<string?>("/value", null);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldHaveJsonValueAt("/path", "value"));
    }

    [Fact]
    public void CustomMessage_ShouldBeIncluded()
    {
        var json = @"{""value"": ""actual""}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldHaveJsonValueAt("/value", "expected", customMessage: customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void EscapedCharacters_ShouldWork()
    {
        var json = @"{""~tilde"": 1, ""slash/prop"": 2}";

        json.ShouldHaveJsonValueAt("/~0tilde", 1);
        json.ShouldHaveJsonValueAt("/slash~1prop", 2);
    }

    [Fact]
    public void ComplexType_ShouldWork()
    {
        var json = @"{""value"": {""name"": ""test""}}";
        var expected = "TeSt";

        json.ShouldHaveJsonValueAt("/value/name", expected, new CustomValueComparer());
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
