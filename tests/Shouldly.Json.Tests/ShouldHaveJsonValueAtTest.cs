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

    [Fact]
    public void NumericTypes_ShouldWork()
    {
        var json = @"{
            ""byteValue"": 255,
            ""sbyteValue"": -128,
            ""shortValue"": 32767,
            ""ushortValue"": 65535
        }";

        json.ShouldHaveJsonValue<byte>("/byteValue", 255);
        json.ShouldHaveJsonValue<sbyte>("/sbyteValue", -128);
        json.ShouldHaveJsonValue<short>("/shortValue", 32767);
        json.ShouldHaveJsonValue<ushort>("/ushortValue", 65535);
    }

    [Fact]
    public void CharacterValues_ShouldWork()
    {
        var json = @"{
            ""charValue"": ""A"",
            ""specialChar"": ""\n"",
            ""unicodeChar"": ""€""
        }";

        json.ShouldHaveJsonValue<char>("/charValue", 'A');
        json.ShouldHaveJsonValue<char>("/specialChar", '\n');
        json.ShouldHaveJsonValue<char>("/unicodeChar", '€');
    }

    [Fact]
    public void GuidValues_ShouldWork()
    {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.Empty;
        var json = $@"{{
            ""guidValue"": ""{guid1}"",
            ""emptyGuid"": ""{guid2}""
        }}";

        json.ShouldHaveJsonValue<Guid>("/guidValue", guid1);
        json.ShouldHaveJsonValue<Guid>("/emptyGuid", guid2);
    }

    [Fact]
    public void TimeSpanValues_ShouldWork()
    {
        var timeSpan1 = TimeSpan.FromHours(2);
        var timeSpan2 = TimeSpan.Zero;
        var timeSpan3 = TimeSpan.FromDays(1);
        var json = $@"{{
            ""duration"": ""{timeSpan1}"",
            ""zeroDuration"": ""{timeSpan2}"",
            ""longDuration"": ""{timeSpan3}""
        }}";

        json.ShouldHaveJsonValue<TimeSpan>("/duration", timeSpan1);
        json.ShouldHaveJsonValue<TimeSpan>("/zeroDuration", timeSpan2);
        json.ShouldHaveJsonValue<TimeSpan>("/longDuration", timeSpan3);
    }

    [Fact]
    public void NullableTypes_ShouldWork()
    {
        var json = @"{
            ""nullInt"": null,
            ""nullGuid"": null,
            ""nullTimeSpan"": null,
            ""nullByte"": null,
            ""hasValueInt"": 42,
            ""hasValueGuid"": ""12345678-1234-5678-9abc-123456789abc""
        }";

        json.ShouldHaveJsonValue<int?>("/nullInt", null);
        json.ShouldHaveJsonValue<Guid?>("/nullGuid", null);
        json.ShouldHaveJsonValue<TimeSpan?>("/nullTimeSpan", null);
        json.ShouldHaveJsonValue<byte?>("/nullByte", null);
        json.ShouldHaveJsonValue<int?>("/hasValueInt", 42);
        json.ShouldHaveJsonValue<Guid?>("/hasValueGuid", new Guid("12345678-1234-5678-9abc-123456789abc"));
    }

    [Fact]
    public void NestedNumericTypes_ShouldWork()
    {
        var json = @"{
            ""data"": {
                ""metrics"": {
                    ""count"": 255,
                    ""offset"": -32768
                }
            }
        }";

        json.ShouldHaveJsonValue<byte>("/data/metrics/count", 255);
        json.ShouldHaveJsonValue<short>("/data/metrics/offset", -32768);
    }

    [Fact]
    public void ArrayOfNewTypes_ShouldWork()
    {
        var guid = Guid.NewGuid();
        var timeSpan = TimeSpan.FromMinutes(30);
        var json = $@"{{
            ""guids"": [""{guid}"", ""{Guid.Empty}""],
            ""timeSpans"": [""{timeSpan}"", ""{TimeSpan.Zero}""],
            ""bytes"": [0, 128, 255]
        }}";

        json.ShouldHaveJsonValue<Guid>("/guids/0", guid);
        json.ShouldHaveJsonValue<Guid>("/guids/1", Guid.Empty);
        json.ShouldHaveJsonValue<TimeSpan>("/timeSpans/0", timeSpan);
        json.ShouldHaveJsonValue<TimeSpan>("/timeSpans/1", TimeSpan.Zero);
        json.ShouldHaveJsonValue<byte>("/bytes/0", 0);
        json.ShouldHaveJsonValue<byte>("/bytes/1", 128);
        json.ShouldHaveJsonValue<byte>("/bytes/2", 255);
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
