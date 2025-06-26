namespace Shouldly;

using System;
using System.Collections.Generic;

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

    [Fact]
    public void NumericComparisons_Float_ShouldWork()
    {
        var json = @"{""value"": 42.5}";
        const string pointer = "/value";
        const float smaller = 41.5f;
        const float larger = 43.5f;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_Double_ShouldWork()
    {
        var json = @"{""value"": 42.5}";
        const string pointer = "/value";
        const double smaller = 41.5d;
        const double larger = 43.5d;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_Decimal_ShouldWork()
    {
        var json = @"{""value"": 42.5}";
        const string pointer = "/value";
        const decimal smaller = 41.5m;
        const decimal larger = 43.5m;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_Long_ShouldWork()
    {
        var json = @"{""value"": 9223372036854775806}";
        const string pointer = "/value";
        const long smaller = 9223372036854775805L;
        const long larger = 9223372036854775807L;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_ULong_ShouldWork()
    {
        var json = @"{""value"": 18446744073709551614}";
        const string pointer = "/value";
        const ulong smaller = 18446744073709551613UL;
        const ulong larger = 18446744073709551615UL;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_Int_ShouldWork()
    {
        var json = @"{""value"": 42}";
        const string pointer = "/value";
        const int smaller = 41;
        const int larger = 43;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_UInt_ShouldWork()
    {
        var json = @"{""value"": 42}";
        const string pointer = "/value";
        const uint smaller = 41;
        const uint larger = 43;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_Short_ShouldWork()
    {
        var json = @"{""value"": 42}";
        const string pointer = "/value";
        const short smaller = 41;
        const short larger = 43;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_UShort_ShouldWork()
    {
        var json = @"{""value"": 42}";
        const string pointer = "/value";
        const ushort smaller = 41;
        const ushort larger = 43;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_Byte_ShouldWork()
    {
        var json = @"{""value"": 42}";
        const string pointer = "/value";
        const byte smaller = 41;
        const byte larger = 43;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void NumericComparisons_SByte_ShouldWork()
    {
        var json = @"{""value"": 42}";
        const string pointer = "/value";
        const sbyte smaller = 41;
        const sbyte larger = 43;

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void CharComparisons_ShouldWork()
    {
        var json = @"{""value"": ""B""}";
        const string pointer = "/value";
        const char smaller = 'A';
        const char larger = 'C';

        json.ShouldHaveJsonValueLessThan(pointer, larger);
        json.ShouldHaveJsonValueGreaterThan(pointer, smaller);
        json.ShouldHaveJsonValueBetween(pointer, smaller, larger);
    }

    [Fact]
    public void GuidComparisons_ShouldWork()
    {
        var guid1 = new Guid("00000000-0000-0000-0000-000000000001");
        var guid2 = new Guid("00000000-0000-0000-0000-000000000002");
        var guid3 = new Guid("00000000-0000-0000-0000-000000000003");
        
        var json = $@"{{""value"": ""{guid2}""}}";
        const string pointer = "/value";

        json.ShouldHaveJsonValueLessThan(pointer, guid3);
        json.ShouldHaveJsonValueGreaterThan(pointer, guid1);
        json.ShouldHaveJsonValueBetween(pointer, guid1, guid3);
    }

    [Fact]
    public void TimeSpanComparisons_ShouldWork()
    {
        var timeSpan1 = TimeSpan.FromMinutes(30);
        var timeSpan2 = TimeSpan.FromHours(1);
        var timeSpan3 = TimeSpan.FromHours(2);
        
        var json = $@"{{""value"": ""{timeSpan2}""}}";
        const string pointer = "/value";

        json.ShouldHaveJsonValueLessThan(pointer, timeSpan3);
        json.ShouldHaveJsonValueGreaterThan(pointer, timeSpan1);
        json.ShouldHaveJsonValueBetween(pointer, timeSpan1, timeSpan3);
    }

    [Fact]
    public void DateTimeComparisons_WithDifferentTimeZones_ShouldWork()
    {
        var utcNow = DateTimeOffset.UtcNow;
        var estTime = TimeZoneInfo.ConvertTime(utcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
        var pstTime = TimeZoneInfo.ConvertTime(utcNow, TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time"));

        var json = $@"{{""utc"": ""{utcNow:O}"", ""est"": ""{estTime:O}"", ""pst"": ""{pstTime:O}""}}";

        // All these times should be equivalent
        json.ShouldHaveJsonDateBetween("/utc", estTime.AddMinutes(-1), estTime.AddMinutes(1));
        json.ShouldHaveJsonDateBetween("/est", utcNow.AddMinutes(-1), utcNow.AddMinutes(1));
        json.ShouldHaveJsonDateBetween("/pst", utcNow.AddMinutes(-1), utcNow.AddMinutes(1));
    }

    [Theory]
    [InlineData(@"{""value"": true}", "/value")]
    [InlineData(@"{""value"": ""42""}", "/value")]
    [InlineData(@"{""value"": []}", "/value")]
    [InlineData(@"{""value"": {}}", "/value")]
    public void NumericComparisons_InvalidTypes_ShouldProvideDescriptiveErrors(string json, string pointer)
    {
        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueLessThan<int>(pointer, 42));

        ex.Message.ShouldContain($"Value at JSON pointer '{pointer}' is not a valid Int32");
    }

    [Theory]
    [InlineData(@"{""value"": ""not-a-date""}", "Value at JSON pointer '/value' is not a valid DateTime")]
    [InlineData(@"{""value"": 42}", "Value at JSON pointer '/value' is not a valid DateTime")]
    public void DateTimeComparisons_InvalidFormats_ShouldProvideDescriptiveErrors(string json, string expectedError)
    {
        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonDateBefore("/value", DateTime.UtcNow));

        ex.Message.ShouldContain(expectedError);
    }

    [Fact]
    public void NumericComparisons_EdgeCases_ShouldWork()
    {
        var json = @"{
            ""zero"": 0,
            ""minInt"": -2147483648,
            ""maxInt"": 2147483647,
            ""epsilon"": 0.000000001,
            ""scientific"": 1.23e-10
        }";

        json.ShouldHaveJsonValueLessThan("/zero", 1);
        json.ShouldHaveJsonValueGreaterThan("/zero", -1);
        json.ShouldHaveJsonValueBetween("/minInt", int.MinValue, int.MaxValue);
        json.ShouldHaveJsonValueBetween("/epsilon", 0.0, 0.000000002);
        json.ShouldHaveJsonValueBetween("/scientific", 1.22e-10, 1.24e-10);
    }

    [Fact]
    public void DateTimeComparisons_EdgeCases_ShouldWork()
    {
        var json = $@"{{
            ""minValue"": ""{DateTime.MinValue:O}"",
            ""maxValue"": ""{DateTime.MaxValue:O}"",
            ""unixEpoch"": ""1970-01-01T00:00:00Z"",
            ""leapYear"": ""2024-02-29T00:00:00Z"",
            ""milliseconds"": ""2024-01-01T00:00:00.123Z""
        }}";

        json.ShouldHaveJsonDateAfter("/unixEpoch", DateTime.UnixEpoch.AddSeconds(-1));
        json.ShouldHaveJsonDateBefore("/unixEpoch", DateTime.UnixEpoch.AddSeconds(1));
        json.ShouldHaveJsonDateBetween("/leapYear", new DateTime(2024, 2, 28), new DateTime(2024, 3, 1));
        json.ShouldHaveJsonDateBetween("/milliseconds", new DateTime(2024, 1, 1, 0, 0, 0, 122), new DateTime(2024, 1, 1, 0, 0, 0, 124));
    }

    [Fact]
    public void NumericComparisons_EdgeCasesForNewTypes_ShouldWork()
    {
        var json = @"{
            ""maxByte"": 255,
            ""minSByte"": -128,
            ""maxShort"": 32767,
            ""maxUShort"": 65535
        }";

        json.ShouldHaveJsonValueBetween("/maxByte", (byte)254, (byte)255);
        json.ShouldHaveJsonValueBetween("/minSByte", (sbyte)-128, (sbyte)-127);
        json.ShouldHaveJsonValueBetween("/maxShort", (short)32766, (short)32767);
        json.ShouldHaveJsonValueBetween("/maxUShort", (ushort)65534, (ushort)65535);
    }

    [Fact]
    public void CharComparisons_EdgeCases_ShouldWork()
    {
        var json = @"{
            ""charA"": ""A"",
            ""charZ"": ""Z"",
            ""digit"": ""5""
        }";

        json.ShouldHaveJsonValueBetween("/charA", 'A', 'B');
        json.ShouldHaveJsonValueBetween("/charZ", 'Y', 'Z');
        json.ShouldHaveJsonValueBetween("/digit", '0', '9');
    }

    [Fact]
    public void TimeSpanComparisons_EdgeCases_ShouldWork()
    {
        var json = $@"{{
            ""zero"": ""{TimeSpan.Zero}"",
            ""oneHour"": ""{TimeSpan.FromHours(1)}"",
            ""oneDay"": ""{TimeSpan.FromDays(1)}""
        }}";

        json.ShouldHaveJsonValueBetween("/zero", TimeSpan.Zero, TimeSpan.FromSeconds(1));
        json.ShouldHaveJsonValueBetween("/oneHour", TimeSpan.FromMinutes(59), TimeSpan.FromMinutes(61));
        json.ShouldHaveJsonValueBetween("/oneDay", TimeSpan.FromHours(23), TimeSpan.FromHours(25));
    }

    [Fact]
    public void NullableValueTypes_ShouldWork()
    {
        var json = @"{
            ""nullInt"": null,
            ""nullGuid"": null,
            ""nullTimeSpan"": null,
            ""nullByte"": null,
            ""hasValueInt"": 42,
            ""hasValueGuid"": ""00000000-0000-0000-0000-000000000001""
        }";

        json.ShouldHaveJsonValue<int?>("/nullInt", null);
        json.ShouldHaveJsonValue<Guid?>("/nullGuid", null);
        json.ShouldHaveJsonValue<TimeSpan?>("/nullTimeSpan", null);
        json.ShouldHaveJsonValue<byte?>("/nullByte", null);
        json.ShouldHaveJsonValue<int?>("/hasValueInt", 42);
        json.ShouldHaveJsonValue<Guid?>("/hasValueGuid", new Guid("00000000-0000-0000-0000-000000000001"));
    }

    [Fact]
    public void GuidComparisons_EdgeCases_ShouldWork()
    {
        var json = $@"{{
            ""emptyGuid"": ""{Guid.Empty}"",
            ""maxGuid"": ""ffffffff-ffff-ffff-ffff-ffffffffffff"",
            ""testGuid"": ""12345678-1234-5678-9abc-123456789abc""
        }}";

        json.ShouldHaveJsonValueGreaterThan("/testGuid", Guid.Empty);
        json.ShouldHaveJsonValueLessThan("/testGuid", new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"));
        json.ShouldHaveJsonValueBetween("/testGuid", 
            new Guid("00000000-0000-0000-0000-000000000000"), 
            new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"));
    }

    [Fact]
    public void InvalidJson_ShouldProvideDescriptiveErrors()
    {
        var invalidJsonCases = new Dictionary<string, string>
        {
            { "{", "Invalid JSON: " },
            { "{'invalid': 'quotes'}", "Invalid JSON: " },
            { @"{""missing"": }", "Invalid JSON: " },
            { @"{""trailing"": ""comma"",}", "Invalid JSON: " },
            { @"[1,]", "Invalid JSON: " }
        };

        foreach (var (invalidJson, expectedError) in invalidJsonCases)
        {
            var ex = Should.Throw<ShouldAssertException>(() => invalidJson.ShouldHaveJsonValueLessThan("/value", 42));

            ex.Message.ShouldContain(expectedError);
        }
    }

    [Fact]
    public void NumericComparisons_WithCustomMessage_ShouldWork()
    {
        var json = @"{""value"": 42}";
        const string customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueLessThan("/value", 40, customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Theory]
    [InlineData("/path/to/value")]
    [InlineData("#/path/to/value")]
    public void DifferentPointerFormats_ShouldWork(string pointer)
    {
        var json = @"{""path"": {""to"": {""value"": 42}}}";
        
        json.ShouldHaveJsonValueLessThan(pointer, 43);
    }
}
