namespace Shouldly;

using System;

public class ShouldHaveJsonValueComparisonTest
{
    [Fact]
    public void NumericComparisons_Integer_ShouldWork()
    {
        var json = @"{""value"": 42, ""nested"": {""value"": 10}}";

        // Less than
        json.ShouldHaveJsonValueLessThan("/value", 50);
        json.ShouldHaveJsonValueLessThan("/nested/value", 42);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueLessThan("/value", 40));

        // Less than or equal
        json.ShouldHaveJsonValueLessThanOrEqualTo("/value", 42);
        json.ShouldHaveJsonValueLessThanOrEqualTo("/value", 50);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueLessThanOrEqualTo("/value", 40));

        // Greater than
        json.ShouldHaveJsonValueGreaterThan("/value", 40);
        json.ShouldHaveJsonValueGreaterThan("/nested/value", 5);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueGreaterThan("/value", 50));

        // Greater than or equal
        json.ShouldHaveJsonValueGreaterThanOrEqualTo("/value", 42);
        json.ShouldHaveJsonValueGreaterThanOrEqualTo("/value", 40);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueGreaterThanOrEqualTo("/value", 50));

        // Between
        json.ShouldHaveJsonValueBetween("/value", 40, 50);
        json.ShouldHaveJsonValueBetween("/value", 42, 42);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueBetween("/value", 43, 50));
    }

    [Fact]
    public void NumericComparisons_Decimal_ShouldWork()
    {
        var json = @"{""value"": 42.5, ""nested"": {""value"": 10.75}}";

        json.ShouldHaveJsonValueLessThan("/value", 43.0m);
        json.ShouldHaveJsonValueGreaterThan("/value", 42.0m);
        json.ShouldHaveJsonValueBetween("/nested/value", 10.0m, 11.0m);
    }

    [Fact]
    public void NumericComparisons_Double_ShouldWork()
    {
        var json = @"{""value"": 42.5, ""nested"": {""value"": 10.75}}";

        json.ShouldHaveJsonValueLessThan("/value", 43.0);
        json.ShouldHaveJsonValueGreaterThan("/value", 42.0);
        json.ShouldHaveJsonValueBetween("/nested/value", 10.0, 11.0);
    }

    [Fact]
    public void DateTimeComparisons_ShouldWork()
    {
        var now = DateTimeOffset.UtcNow;
        var past = now.AddDays(-1);
        var future = now.AddDays(1);
        
        var json = $@"{{
            ""current"": ""{now:O}"",
            ""nested"": {{
                ""past"": ""{past:O}"",
                ""future"": ""{future:O}""
            }}
        }}";

        // Before
        json.ShouldHaveJsonDateBefore("/current", future);
        json.ShouldHaveJsonDateBefore("/nested/past", future);
        json.ShouldHaveJsonDateBefore("/nested/past", now);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore("/current", now));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore("/current", past));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore("/nested/past", past));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore("/nested/future", past));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore("/nested/future", now));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore("/nested/future", future));

        // Before or at
        json.ShouldHaveJsonDateBeforeOrEqualTo("/current", future);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/current", now);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/nested/past", future);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/nested/past", now);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/nested/past", past);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/nested/future", future);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBeforeOrEqualTo("/current", past));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBeforeOrEqualTo("/nested/future", past));

        // After
        json.ShouldHaveJsonDateAfter("/current", past);
        json.ShouldHaveJsonDateAfter("/nested/future", past);
        json.ShouldHaveJsonDateAfter("/nested/future", now);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfter("/current", now));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfter("/current", future));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfter("/nested/past", past));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfter("/nested/past", now));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfter("/nested/past", future));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfter("/nested/future", future));

        // After or at
        json.ShouldHaveJsonDateAfterOrEqualTo("/current", past);
        json.ShouldHaveJsonDateAfterOrEqualTo("/current", now);
        json.ShouldHaveJsonDateAfterOrEqualTo("/nested/past", past);
        json.ShouldHaveJsonDateAfterOrEqualTo("/nested/future", past);
        json.ShouldHaveJsonDateAfterOrEqualTo("/nested/future", now);
        json.ShouldHaveJsonDateAfterOrEqualTo("/nested/future", future);
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfterOrEqualTo("/current", future));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfterOrEqualTo("/nested/past", now));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateAfterOrEqualTo("/nested/past", future));

        // Between
        json.ShouldHaveJsonDateBetween("/current", past, future);
        json.ShouldHaveJsonDateBetween("/current", past, now.AddSeconds(1));
        json.ShouldHaveJsonDateBetween("/current", now, now.AddSeconds(1));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBetween("/current", past, now));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBetween("/current", past, past.AddHours(1)));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBetween("/current", future, future.AddHours(1)));
    }

    [Fact]
    public void DateTimeComparisons_WithTimeZones_ShouldWork()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var estNow = utcNow.ToOffset(TimeSpan.FromHours(-5));
        
        var json = $@"{{""value"": ""{estNow:O}""}}";

        // Times should be equivalent regardless of timezone
        json.ShouldHaveJsonDateBetween("/value", utcNow.AddMinutes(-1), utcNow.AddMinutes(1));
    }

    [Fact]
    public void DateTimeComparisons_WithDateTime_ShouldWork()
    {
        var now = DateTime.UtcNow;
        var past = now.AddDays(-1);
        var future = now.AddDays(1);
        
        var json = $@"{{
            ""current"": ""{now:O}"",
            ""nested"": {{
                ""value"": ""{past:O}""
            }}
        }}";

        json.ShouldHaveJsonDateBefore("/current", future);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/current", now);
        json.ShouldHaveJsonDateAfter("/current", past);
        json.ShouldHaveJsonDateAfterOrEqualTo("/current", now);
        json.ShouldHaveJsonDateBetween("/current", past, future);
    }

    [Fact]
    public void DateTimeComparisons_WithDateTimeOffset_ShouldWork()
    {
        var now = DateTimeOffset.UtcNow;
        var past = now.AddDays(-1);
        var future = now.AddDays(1);
        
        var json = $@"{{
            ""current"": ""{now:O}""
        }}";

        json.ShouldHaveJsonDateBefore("/current", future);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/current", now);
        json.ShouldHaveJsonDateAfter("/current", past);
        json.ShouldHaveJsonDateAfterOrEqualTo("/current", now);
        json.ShouldHaveJsonDateBetween("/current", past, future);
    }

    [Fact]
    public void DateTimeComparisons_WithDateOnly_ShouldWork()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var yesterday = today.AddDays(-1);
        var tomorrow = today.AddDays(1);
        
        var json = $@"{{
            ""current"": ""{today:O}""
        }}";

        json.ShouldHaveJsonDateBefore("/current", tomorrow);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/current", today);
        json.ShouldHaveJsonDateAfter("/current", yesterday);
        json.ShouldHaveJsonDateAfterOrEqualTo("/current", today);
        json.ShouldHaveJsonDateBetween("/current", yesterday, tomorrow);
    }

    [Fact]
    public void DateTimeComparisons_WithTimeOnly_ShouldWork()
    {
        var now = TimeOnly.FromDateTime(DateTime.UtcNow);
        var earlier = now.AddHours(-1);
        var later = now.AddHours(1);
        
        var json = $@"{{
            ""current"": ""{now:O}""
        }}";

        json.ShouldHaveJsonDateBefore("/current", later);
        json.ShouldHaveJsonDateBeforeOrEqualTo("/current", now);
        json.ShouldHaveJsonDateAfter("/current", earlier);
        json.ShouldHaveJsonDateAfterOrEqualTo("/current", now);
        json.ShouldHaveJsonDateBetween("/current", earlier, later);
    }

    [Theory]
    [InlineData(@"{""value"": ""not a number""}", "/value")]
    [InlineData(@"{""value"": true}", "/value")]
    [InlineData(@"{""value"": null}", "/value")]
    public void NumericComparisons_WithInvalidTypes_ShouldThrow(string json, string pointer)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueLessThan(pointer, 42));
    }

    [Theory]
    [InlineData(@"{""value"": ""not a date""}", "/value")]
    [InlineData(@"{""value"": 42}", "/value")]
    [InlineData(@"{""value"": null}", "/value")]
    public void DateComparisons_WithInvalidTypes_ShouldThrow(string json, string pointer)
    {
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore(pointer, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Comparisons_WithInvalidPointer_ShouldThrow()
    {
        var json = @"{""value"": 42}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueLessThan("/invalid/path", 50));
        
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore("/invalid/path", DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Comparisons_WithCustomMessage_ShouldIncludeMessage()
    {
        var json = @"{""value"": 42}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueLessThan("/value", 40, customMessage));
        
        ex.Message.ShouldContain(customMessage);
    }
}
