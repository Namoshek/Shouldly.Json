using System;
using System.Collections.Generic;

namespace Shouldly;

/// <summary>
/// Covers the null-handling semantics of <see cref="JsonHelper"/> that the assertions rely on:
/// a JSON null must never be reported as a successful default for a non-nullable value type,
/// and a document whose root is literally <c>null</c> must be recognized at the root pointers.
/// </summary>
public class JsonHelperNullSemanticsTest
{
    // Non-nullable value type vs. JSON null

    [Fact]
    public void ShouldHaveJsonValue_Int_WithNull_ShouldNotTreatAsDefault()
    {
        var json = @"{""value"": null}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValue("/value", 0));

        ex.Message.ShouldContain("is null, not a valid Int32");
    }

    [Fact]
    public void ShouldHaveJsonValueOneOf_Int_WithNull_ShouldNotMatchZero()
    {
        var json = @"{""value"": null}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueOneOf("/value", new[] { 0 }));

        ex.Message.ShouldContain("is null, not a valid Int32");
    }

    [Fact]
    public void ShouldHaveJsonValue_OneOf_Int_WithRealZero_ShouldStillMatch()
    {
        @"{""value"": 0}".ShouldHaveJsonValueOneOf("/value", new[] { 0 });
    }

    [Fact]
    public void ShouldNotHaveJsonValue_Int_WithNull_ShouldThrowWithoutClaimingZero()
    {
        var json = @"{""value"": null}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValue("/value", 0));

        ex.Message.ShouldContain("is null, not a valid Int32");
        ex.Message.ShouldNotContain("not '0'");
    }

    [Fact]
    public void ShouldNotHaveJsonValueOneOf_Int_WithNull_ShouldThrowWithoutClaimingMembership()
    {
        var json = @"{""value"": null}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueOneOf("/value", new[] { 0 }));

        ex.Message.ShouldContain("is null, not a valid Int32");
    }

    [Fact]
    public void ShouldHaveJsonValue_NullableInt_WithNull_ShouldSucceed()
    {
        // Default equality for nullable value types matches null to null.
        @"{""value"": null}".ShouldHaveJsonValue<int?>("/value", null);
    }

    [Fact]
    public void ShouldHaveJsonValue_String_WithNull_ShouldStillSucceed()
    {
        @"{""value"": null}".ShouldHaveJsonValue<string?>("/value", null);
    }

    // Root document that is literally null

    [Theory]
    [InlineData("")]
    [InlineData("#")]
    public void ShouldHaveJsonValueOneOf_RootNull_WithNullableNullMember_ShouldSucceed(string pointer)
    {
        "null".ShouldHaveJsonValueOneOf<string?>(pointer, new List<string?> { null, "a" });
    }

    [Fact]
    public void ShouldHaveJsonValue_RootNull_WithNullableNullMember_ShouldSucceed()
    {
        "null".ShouldHaveJsonValue<string?>("", null);
    }

    [Fact]
    public void ShouldHaveJsonValue_RootNull_PathBelowNull_ShouldReportNotFound()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "null".ShouldHaveJsonValue<string?>("/nested", null));

        ex.Message.ShouldContain("No value found at JSON pointer '/nested'");
    }

    [Fact]
    public void ShouldHaveJsonValue_RootNull_InvalidPointer_ShouldReportInvalidPointer()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "null".ShouldHaveJsonValue<string?>("bad~pointer", null));

        ex.Message.ShouldContain("Invalid JSON pointer");
    }

    [Fact]
    public void ShouldNotHaveJsonValueOneOf_RootNull_PathBelowNull_ShouldNotThrow()
    {
        "null".ShouldNotHaveJsonValueOneOf<string?>("/nested", new List<string?> { "a" });
    }

    // Nested null remains equivalent to the root-null handling.

    [Fact]
    public void ShouldHaveJsonValueOneOf_NestedNull_ShouldMatchNullableNullMember()
    {
        @"{""outer"": {""inner"": null}}".ShouldHaveJsonValueOneOf<string?>(
            "/outer/inner",
            new List<string?> { null, "a" });
    }

    // Validation assertions must keep working while disposing the parsed document.

    [Fact]
    public void ShouldBeValidJson_LargeValidDocument_ShouldPass()
    {
        var items = new List<string>();
        for (var i = 0; i < 2000; i++)
        {
            items.Add($@"{{""id"": {i}}}");
        }

        $"[{string.Join(",", items)}]".ShouldBeValidJson();
    }

    [Fact]
    public void ShouldBeValidJson_InvalidDocument_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "{invalid json}".ShouldBeValidJson());

        ex.Message.ShouldContain("String should be valid JSON");
    }

    [Fact]
    public void ShouldNotBeValidJson_ValidDocument_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => @"{""a"": 1}".ShouldNotBeValidJson());

        ex.Message.ShouldContain("String should not be valid JSON");
    }

    [Fact]
    public void ShouldNotBeValidJson_InvalidDocument_ShouldPass()
    {
        "{invalid json}".ShouldNotBeValidJson();
    }
}
