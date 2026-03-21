namespace Shouldly;

using System;

public class ShouldAllHaveJsonDateTest
{
    private readonly string _json = @"{""events"": [
        {""date"": ""1990-01-15""},
        {""date"": ""1990-06-20""},
        {""date"": ""1990-12-31""}
    ]}";

    // ShouldAllHaveJsonDateBefore

    [Fact]
    public void DateBefore_AllDatesBefore_ShouldNotThrow()
    {
        _json.ShouldAllHaveJsonDateBefore("/events", "/date", new DateTime(1991, 1, 1));
    }

    [Fact]
    public void DateBefore_SomeDatesNotBefore_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            _json.ShouldAllHaveJsonDateBefore("/events", "/date", new DateTime(1990, 6, 20)));

        ex.Message.ShouldContain("before");
        ex.Message.ShouldContain("index 1");
        ex.Message.ShouldContain("index 2");
    }

    [Fact]
    public void DateBefore_EmptyArray_ShouldNotThrow()
    {
        var json = @"{""events"": []}";
        json.ShouldAllHaveJsonDateBefore("/events", "/date", new DateTime(2025, 1, 1));
    }

    [Fact]
    public void DateBefore_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";
        var ex = Should.Throw<ShouldAssertException>(() =>
            _json.ShouldAllHaveJsonDateBefore("/events", "/date", new DateTime(1989, 1, 1), customMessage));
        ex.Message.ShouldContain(customMessage);
    }

    // ShouldAllHaveJsonDateBeforeOrEqualTo

    [Fact]
    public void DateBeforeOrEqualTo_AllDatesPass_ShouldNotThrow()
    {
        _json.ShouldAllHaveJsonDateBeforeOrEqualTo("/events", "/date", new DateTime(1990, 12, 31));
    }

    [Fact]
    public void DateBeforeOrEqualTo_SomeDatesTooLate_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            _json.ShouldAllHaveJsonDateBeforeOrEqualTo("/events", "/date", new DateTime(1990, 6, 19)));

        ex.Message.ShouldContain("before or equal to");
        ex.Message.ShouldContain("index 1");
        ex.Message.ShouldContain("index 2");
    }

    // ShouldAllHaveJsonDateAfter

    [Fact]
    public void DateAfter_AllDatesAfter_ShouldNotThrow()
    {
        _json.ShouldAllHaveJsonDateAfter("/events", "/date", new DateTime(1989, 12, 31));
    }

    [Fact]
    public void DateAfter_SomeDatesNotAfter_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            _json.ShouldAllHaveJsonDateAfter("/events", "/date", new DateTime(1990, 6, 20)));

        ex.Message.ShouldContain("after");
        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("index 1");
    }

    [Fact]
    public void DateAfter_EmptyArray_ShouldNotThrow()
    {
        var json = @"{""events"": []}";
        json.ShouldAllHaveJsonDateAfter("/events", "/date", new DateTime(1980, 1, 1));
    }

    [Fact]
    public void DateAfter_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";
        var ex = Should.Throw<ShouldAssertException>(() =>
            _json.ShouldAllHaveJsonDateAfter("/events", "/date", new DateTime(2025, 1, 1), customMessage));
        ex.Message.ShouldContain(customMessage);
    }

    // ShouldAllHaveJsonDateAfterOrEqualTo

    [Fact]
    public void DateAfterOrEqualTo_AllDatesPass_ShouldNotThrow()
    {
        _json.ShouldAllHaveJsonDateAfterOrEqualTo("/events", "/date", new DateTime(1990, 1, 15));
    }

    [Fact]
    public void DateAfterOrEqualTo_SomeDatesTooEarly_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            _json.ShouldAllHaveJsonDateAfterOrEqualTo("/events", "/date", new DateTime(1990, 6, 21)));

        ex.Message.ShouldContain("after or equal to");
        ex.Message.ShouldContain("index 0");
        ex.Message.ShouldContain("index 1");
    }

    // ShouldAllHaveJsonDateBetween

    [Fact]
    public void DateBetween_AllDatesInRange_ShouldNotThrow()
    {
        _json.ShouldAllHaveJsonDateBetween("/events", "/date", new DateTime(1990, 1, 1), new DateTime(1991, 1, 1));
    }

    [Fact]
    public void DateBetween_SomeDatesOutOfRange_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() =>
            _json.ShouldAllHaveJsonDateBetween("/events", "/date", new DateTime(1990, 2, 1), new DateTime(1990, 12, 1)));

        ex.Message.ShouldContain("between");
        ex.Message.ShouldContain("inclusive");
        ex.Message.ShouldContain("exclusive");
    }

    [Fact]
    public void DateBetween_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";
        var ex = Should.Throw<ShouldAssertException>(() =>
            _json.ShouldAllHaveJsonDateBetween("/events", "/date",
                new DateTime(2000, 1, 1), new DateTime(2001, 1, 1), customMessage));
        ex.Message.ShouldContain(customMessage);
    }

    // Invalid input errors

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() =>
            invalidJson.ShouldAllHaveJsonDateBefore("/events", "/date", new DateTime(2025, 1, 1)));
    }

    [Fact]
    public void MissingDateProperty_ShouldThrow()
    {
        var json = @"{""events"": [{""name"": ""Event 1""}]}";

        Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonDateBefore("/events", "/date", new DateTime(2025, 1, 1)));
    }
}
