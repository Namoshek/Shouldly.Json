namespace Shouldly;

using System;

public class ShouldAllHaveJsonValueComparisonTest
{
    private const string UsersJson = @"{""users"": [{""age"": 25}, {""age"": 30}, {""age"": 35}]}";

    // ShouldAllHaveJsonValueLessThan

    [Fact]
    public void LessThan_AllValuesLess_ShouldNotThrow()
    {
        UsersJson.ShouldAllHaveJsonValueLessThan("/users", "/age", 40);
    }

    [Fact]
    public void LessThan_SomeValuesNotLess_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            UsersJson.ShouldAllHaveJsonValueLessThan("/users", "/age", 30));

        ex.Message.ShouldContain("less than 30");
        ex.Message.ShouldContain("index 1");
        ex.Message.ShouldContain("index 2");
    }

    [Fact]
    public void LessThan_EmptyArray_ShouldNotThrow()
    {
        var json = @"{""users"": []}";
        json.ShouldAllHaveJsonValueLessThan("/users", "/age", 100);
    }

    [Fact]
    public void LessThan_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";
        var ex = Should.Throw<ShouldAssertException>(() =>
            UsersJson.ShouldAllHaveJsonValueLessThan("/users", "/age", 20, customMessage));
        ex.Message.ShouldContain(customMessage);
    }

    // ShouldAllHaveJsonValueLessThanOrEqualTo

    [Fact]
    public void LessThanOrEqualTo_AllValuesPass_ShouldNotThrow()
    {
        UsersJson.ShouldAllHaveJsonValueLessThanOrEqualTo("/users", "/age", 35);
    }

    [Fact]
    public void LessThanOrEqualTo_SomeValuesTooHigh_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            UsersJson.ShouldAllHaveJsonValueLessThanOrEqualTo("/users", "/age", 29));

        ex.Message.ShouldContain("less than or equal to 29");
        ex.Message.ShouldContain("index 1");
        ex.Message.ShouldContain("index 2");
    }

    // ShouldAllHaveJsonValueGreaterThan

    [Fact]
    public void GreaterThan_AllValuesGreater_ShouldNotThrow()
    {
        UsersJson.ShouldAllHaveJsonValueGreaterThan("/users", "/age", 20);
    }

    [Fact]
    public void GreaterThan_SomeValuesNotGreater_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            UsersJson.ShouldAllHaveJsonValueGreaterThan("/users", "/age", 30));

        ex.Message.ShouldContain("greater than 30");
        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("index 1");
    }

    [Fact]
    public void GreaterThan_EmptyArray_ShouldNotThrow()
    {
        var json = @"{""users"": []}";
        json.ShouldAllHaveJsonValueGreaterThan("/users", "/age", 0);
    }

    [Fact]
    public void GreaterThan_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";
        var ex = Should.Throw<ShouldAssertException>(() =>
            UsersJson.ShouldAllHaveJsonValueGreaterThan("/users", "/age", 100, customMessage));
        ex.Message.ShouldContain(customMessage);
    }

    // ShouldAllHaveJsonValueGreaterThanOrEqualTo

    [Fact]
    public void GreaterThanOrEqualTo_AllValuesPass_ShouldNotThrow()
    {
        UsersJson.ShouldAllHaveJsonValueGreaterThanOrEqualTo("/users", "/age", 25);
    }

    [Fact]
    public void GreaterThanOrEqualTo_SomeValuesTooLow_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            UsersJson.ShouldAllHaveJsonValueGreaterThanOrEqualTo("/users", "/age", 30));

        ex.Message.ShouldContain("greater than or equal to 30");
        ex.Message.ShouldContain("index 0");
    }

    // ShouldAllHaveJsonValueBetween

    [Fact]
    public void Between_AllValuesBetween_ShouldNotThrow()
    {
        UsersJson.ShouldAllHaveJsonValueBetween("/users", "/age", 20, 40);
    }

    [Fact]
    public void Between_AllValuesWithSameBoundary_ShouldNotThrow()
    {
        UsersJson.ShouldAllHaveJsonValueBetween("/users", "/age", 25, 35);
    }

    [Fact]
    public void Between_SomeValuesOutOfRange_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            UsersJson.ShouldAllHaveJsonValueBetween("/users", "/age", 26, 34));

        ex.Message.ShouldContain("between 26 and 34");
    }

    [Fact]
    public void Between_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";
        var ex = Should.Throw<ShouldAssertException>(() =>
            UsersJson.ShouldAllHaveJsonValueBetween("/users", "/age", 0, 10, customMessage));
        ex.Message.ShouldContain(customMessage);
    }

    // MissingProperty errors

    [Fact]
    public void MissingProperty_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}]}";

        Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValueLessThan("/users", "/age", 100));
    }

    // Invalid JSON/pointer errors

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() =>
            invalidJson.ShouldAllHaveJsonValueLessThan("/users", "/age", 100));
    }

    [Fact]
    public void ArrayPointerToNonArray_ShouldThrow()
    {
        var json = @"{""user"": {""age"": 25}}";

        Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonValueLessThan("/user", "/age", 100));
    }

    // Double values

    [Fact]
    public void DoubleValues_ShouldWork()
    {
        var json = @"{""items"": [{""price"": 9.99}, {""price"": 14.99}]}";

        json.ShouldAllHaveJsonValueLessThan("/items", "/price", 20.0);
        json.ShouldAllHaveJsonValueGreaterThan("/items", "/price", 5.0);
        json.ShouldAllHaveJsonValueBetween("/items", "/price", 5.0, 20.0);
    }
}
