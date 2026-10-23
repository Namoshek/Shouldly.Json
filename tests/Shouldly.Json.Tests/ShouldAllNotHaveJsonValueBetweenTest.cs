using System;

namespace Shouldly;

public class ShouldAllNotHaveJsonValueBetweenTest
{
    private const string NumbersJson = @"{""items"": [{""value"": 1}, {""value"": 5}, {""value"": 10}]}";

    private const string DatesJson = @"{""items"": [{""date"": ""2025-01-01T00:00:00Z""}, {""date"": ""2025-06-15T00:00:00Z""}, {""date"": ""2025-12-01T00:00:00Z""}]}";

    [Fact]
    public void ShouldAllNotHaveJsonValueBetween_WhenNoneInRange_ShouldNotThrow()
    {
        NumbersJson.ShouldAllNotHaveJsonValueBetween("/items", "/value", 20, 30);
    }

    [Fact]
    public void ShouldAllNotHaveJsonValueBetween_WhenOneInRange_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => NumbersJson.ShouldAllNotHaveJsonValueBetween("/items", "/value", 4, 6));

        ex.Message.ShouldContain("should have value at '/value' between 4 and 6");
        ex.Message.ShouldContain("matching values: index 1: 5");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValueBetween_BoundariesAreInclusive()
    {
        var ex = Should.Throw<ShouldAssertException>(() => NumbersJson.ShouldAllNotHaveJsonValueBetween("/items", "/value", 1, 10));

        ex.Message.ShouldContain("index 0: 1");
        ex.Message.ShouldContain("index 2: 10");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValueBetween_NonArrayPointer_ShouldThrow()
    {
        var json = @"{""items"": ""not an array""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllNotHaveJsonValueBetween("/items", "/value", 1, 2));

        ex.Message.ShouldContain("is not an array");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValueBetween_MissingProperty_ShouldThrow()
    {
        var json = @"{""items"": [{""id"": 1}, {""id"": 2}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllNotHaveJsonValueBetween("/items", "/value", 50, 60));

        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("index 1");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValueBetween_TypeMismatch_ShouldThrow()
    {
        var json = @"{""items"": [{""value"": ""text""}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllNotHaveJsonValueBetween("/items", "/value", 1, 2));

        ex.Message.ShouldContain("index 0");
    }

    [Fact]
    public void ShouldAllNotHaveJsonValueBetween_EmptyArray_ShouldNotThrow()
    {
        @"{""items"": []}".ShouldAllNotHaveJsonValueBetween("/items", "/value", 1, 2);
    }

    [Fact]
    public void ShouldAllNotHaveJsonValueBetween_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => NumbersJson.ShouldAllNotHaveJsonValueBetween("/items", "/value", 4, 6, customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    // ShouldAllNotHaveJsonDateBetween

    [Fact]
    public void ShouldAllNotHaveJsonDateBetween_WhenNoneInRange_ShouldNotThrow()
    {
        DatesJson.ShouldAllNotHaveJsonDateBetween(
            "/items",
            "/date",
            new DateTimeOffset(2025, 3, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 5, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void ShouldAllNotHaveJsonDateBetween_WhenOneInRange_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => DatesJson.ShouldAllNotHaveJsonDateBetween(
            "/items",
            "/date",
            new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero)));

        ex.Message.ShouldContain("should have date at '/date' between");
        ex.Message.ShouldContain("(inclusive)");
        ex.Message.ShouldContain("(exclusive)");
        ex.Message.ShouldContain("index 1");
    }

    [Fact]
    public void ShouldAllNotHaveJsonDateBetween_ExclusiveEndIsAllowed()
    {
        DatesJson.ShouldAllNotHaveJsonDateBetween(
            "/items",
            "/date",
            new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void ShouldAllNotHaveJsonDateBetween_InvalidDate_ShouldThrow()
    {
        var json = @"{""items"": [{""date"": ""not a date""}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllNotHaveJsonDateBetween(
            "/items",
            "/date",
            new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 2, 1, 0, 0, 0, TimeSpan.Zero)));

        ex.Message.ShouldContain("index 0");
    }

    [Fact]
    public void ShouldAllNotHaveJsonDateBetween_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => DatesJson.ShouldAllNotHaveJsonDateBetween(
            "/items",
            "/date",
            new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 11, 1, 0, 0, 0, TimeSpan.Zero),
            customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
