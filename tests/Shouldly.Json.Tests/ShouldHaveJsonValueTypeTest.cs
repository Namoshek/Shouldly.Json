namespace Shouldly;

using System.Text.Json;

public class ShouldHaveJsonValueTypeTest
{
    private const string SampleJson = @"{""obj"": {""a"": 1}, ""arr"": [1, 2], ""str"": ""hi"", ""num"": 42, ""bool"": true, ""nil"": null}";

    // Generic kind assertion

    [Theory]
    [InlineData("/obj", JsonValueKind.Object)]
    [InlineData("/arr", JsonValueKind.Array)]
    [InlineData("/str", JsonValueKind.String)]
    [InlineData("/num", JsonValueKind.Number)]
    [InlineData("/bool", JsonValueKind.True)]
    [InlineData("/nil", JsonValueKind.Null)]
    public void ShouldHaveJsonValueType_WithMatchingKind_ShouldNotThrow(string pointer, JsonValueKind expectedKind)
    {
        SampleJson.ShouldHaveJsonValueType(pointer, expectedKind);
    }

    [Fact]
    public void ShouldHaveJsonValueType_WithMismatchingKind_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldHaveJsonValueType("/num", JsonValueKind.String));

        ex.Message.ShouldContain("should be string but was number");
    }

    [Fact]
    public void ShouldHaveJsonValueType_BooleanAssertion_MatchesFalse()
    {
        var json = @"{""flag"": false}";

        json.ShouldHaveJsonValueType("/flag", JsonValueKind.False);
    }

    [Fact]
    public void ShouldHaveJsonValueType_MissingPointer_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldHaveJsonValueType("/missing", JsonValueKind.String));

        ex.Message.ShouldContain("No value found at JSON pointer '/missing'");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("~invalid")]
    public void ShouldHaveJsonValueType_InvalidPointer_ShouldThrow(string pointer)
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldHaveJsonValueType(pointer, JsonValueKind.String));

        ex.Message.ShouldContain("Invalid JSON pointer");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void ShouldHaveJsonValueType_InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldHaveJsonValueType("/str", JsonValueKind.String));
    }

    [Fact]
    public void ShouldHaveJsonValueType_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldHaveJsonValueType("/num", JsonValueKind.String, customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void ShouldNotHaveJsonValueType_WithDifferentKind_ShouldNotThrow()
    {
        SampleJson.ShouldNotHaveJsonValueType("/num", JsonValueKind.String);
    }

    [Fact]
    public void ShouldNotHaveJsonValueType_WithMatchingKind_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldNotHaveJsonValueType("/num", JsonValueKind.Number));

        ex.Message.ShouldContain("should not be number");
    }

    // Named conveniences

    [Fact]
    public void ShouldBeJsonObjectAt_WithObject_ShouldNotThrow()
    {
        SampleJson.ShouldBeJsonObjectAt("/obj");
    }

    [Fact]
    public void ShouldBeJsonObjectAt_WithNonObject_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldBeJsonObjectAt("/num"));

        ex.Message.ShouldContain("should be object but was number");
    }

    [Fact]
    public void ShouldBeJsonObjectAt_WithNull_ShouldThrow()
    {
        var json = @"{""nil"": null}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonObjectAt("/nil"));

        ex.Message.ShouldContain("should be object but was null");
    }

    [Fact]
    public void ShouldNotBeJsonObjectAt_WithNonObject_ShouldNotThrow()
    {
        SampleJson.ShouldNotBeJsonObjectAt("/num");
    }

    [Fact]
    public void ShouldNotBeJsonObjectAt_WithObject_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldNotBeJsonObjectAt("/obj"));

        ex.Message.ShouldContain("should not be object");
    }

    [Fact]
    public void ShouldBeJsonArrayAt_WithArray_ShouldNotThrow()
    {
        SampleJson.ShouldBeJsonArrayAt("/arr");
    }

    [Fact]
    public void ShouldBeJsonArrayAt_WithNonArray_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldBeJsonArrayAt("/obj"));

        ex.Message.ShouldContain("should be array but was object");
    }

    [Fact]
    public void ShouldNotBeJsonArrayAt_WithNonArray_ShouldNotThrow()
    {
        SampleJson.ShouldNotBeJsonArrayAt("/obj");
    }

    [Fact]
    public void ShouldNotBeJsonArrayAt_WithArray_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldNotBeJsonArrayAt("/arr"));

        ex.Message.ShouldContain("should not be array");
    }

    [Fact]
    public void ShouldBeJsonStringAt_WithString_ShouldNotThrow()
    {
        SampleJson.ShouldBeJsonStringAt("/str");
    }

    [Fact]
    public void ShouldBeJsonStringAt_WithNonString_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldBeJsonStringAt("/num"));

        ex.Message.ShouldContain("should be string but was number");
    }

    [Fact]
    public void ShouldNotBeJsonStringAt_WithNonString_ShouldNotThrow()
    {
        SampleJson.ShouldNotBeJsonStringAt("/num");
    }

    [Fact]
    public void ShouldNotBeJsonStringAt_WithString_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldNotBeJsonStringAt("/str"));

        ex.Message.ShouldContain("should not be string");
    }

    [Fact]
    public void ShouldBeJsonNumberAt_WithNumber_ShouldNotThrow()
    {
        SampleJson.ShouldBeJsonNumberAt("/num");
    }

    [Fact]
    public void ShouldBeJsonNumberAt_WithNonNumber_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldBeJsonNumberAt("/str"));

        ex.Message.ShouldContain("should be number but was string");
    }

    [Fact]
    public void ShouldNotBeJsonNumberAt_WithNonNumber_ShouldNotThrow()
    {
        SampleJson.ShouldNotBeJsonNumberAt("/str");
    }

    [Fact]
    public void ShouldNotBeJsonNumberAt_WithNumber_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldNotBeJsonNumberAt("/num"));

        ex.Message.ShouldContain("should not be number");
    }

    [Theory]
    [InlineData(@"{""flag"": true}")]
    [InlineData(@"{""flag"": false}")]
    public void ShouldBeJsonBooleanAt_WithBoolean_ShouldNotThrow(string json)
    {
        json.ShouldBeJsonBooleanAt("/flag");
    }

    [Fact]
    public void ShouldBeJsonBooleanAt_WithNonBoolean_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldBeJsonBooleanAt("/num"));

        ex.Message.ShouldContain("should be boolean but was number");
    }

    [Fact]
    public void ShouldNotBeJsonBooleanAt_WithNonBoolean_ShouldNotThrow()
    {
        SampleJson.ShouldNotBeJsonBooleanAt("/num");
    }

    [Fact]
    public void ShouldNotBeJsonBooleanAt_WithBoolean_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldNotBeJsonBooleanAt("/bool"));

        ex.Message.ShouldContain("should not be boolean");
    }

    [Fact]
    public void ShouldBeJsonObjectAt_WithUriFragmentPointer_ShouldNotThrow()
    {
        SampleJson.ShouldBeJsonObjectAt("#/obj");
    }

    // Null value assertions

    [Fact]
    public void ShouldHaveJsonNullValue_WithNullProperty_ShouldNotThrow()
    {
        SampleJson.ShouldHaveJsonNullValue("/nil");
    }

    [Fact]
    public void ShouldHaveJsonNullValue_WithValueProperty_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldHaveJsonNullValue("/num"));

        ex.Message.ShouldContain("should be null but was number");
    }

    [Fact]
    public void ShouldHaveJsonNullValue_MissingProperty_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldHaveJsonNullValue("/missing"));

        ex.Message.ShouldContain("No value found at JSON pointer '/missing'");
    }

    [Fact]
    public void ShouldHaveJsonNullValue_WithDefaultPointer_ShouldWork()
    {
        "null".ShouldHaveJsonNullValue();
    }

    [Fact]
    public void ShouldNotHaveJsonNullValue_WithValueProperty_ShouldNotThrow()
    {
        SampleJson.ShouldNotHaveJsonNullValue("/num");
    }

    [Fact]
    public void ShouldNotHaveJsonNullValue_WithNullProperty_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldNotHaveJsonNullValue("/nil"));

        ex.Message.ShouldContain("should not be null");
    }

    [Fact]
    public void ShouldNotHaveJsonNullValue_MissingProperty_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => SampleJson.ShouldNotHaveJsonNullValue("/missing"));

        ex.Message.ShouldContain("No value found at JSON pointer '/missing'");
    }

    [Fact]
    public void ShouldNotHaveJsonNullValue_DistinguishesNullFromPresentValue()
    {
        var json = @"{""a"": null, ""b"": 0}";

        json.ShouldHaveJsonNullValue("/a");
        json.ShouldNotHaveJsonNullValue("/b");
    }
}
