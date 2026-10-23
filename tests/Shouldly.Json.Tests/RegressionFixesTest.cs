namespace Shouldly;

public class RegressionFixesTest
{
    [Fact]
    public void ShouldNotHaveJsonArrayCount_RootArrayWithDefaultPointer_ShouldNotThrow()
    {
        "[1, 2, 3]".ShouldNotHaveJsonArrayCount(2);
    }

    [Fact]
    public void ShouldNotHaveJsonArrayCount_RootArrayWithDefaultPointer_MatchingCount_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "[1, 2, 3]".ShouldNotHaveJsonArrayCount(3));

        ex.Message.ShouldContain("should not have 3 elements");
    }

    [Fact]
    public void ArrayCount_DefaultPointer_RootArrayIsTheInversePair()
    {
        "[1, 2, 3]".ShouldHaveJsonArrayCount(3);
        "[1, 2, 3]".ShouldNotHaveJsonArrayCount(2);

        Should.Throw<ShouldAssertException>(() => "[1, 2, 3]".ShouldHaveJsonArrayCount(2));
        Should.Throw<ShouldAssertException>(() => "[1, 2, 3]".ShouldNotHaveJsonArrayCount(3));
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_ReportsDifferencePath()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{""name"": ""Jane"", ""age"": 30}";

        var ex = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));

        ex.Message.ShouldContain("JSON value mismatch at path '/name'");
    }

    [Fact]
    public void ShouldNotBeSemanticallySameJson_WithArrayOrderDifference_ShouldNotThrow()
    {
        var actual = @"{""items"": [1, 2, 3]}";
        var expected = @"{""items"": [3, 2, 1]}";

        actual.ShouldNotBeSemanticallySameJson(expected);
    }

    [Fact]
    public void ShouldNotBeSemanticallySameJson_EquivalentNumbers_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => @"{""n"": 42}".ShouldNotBeSemanticallySameJson(@"{""n"": 42.0}"));

        ex.Message.ShouldContain("should not be semantically the same");
    }

    [Fact]
    public void ShouldNotBeSemanticallySameJson_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => @"{""n"": 1}".ShouldNotBeSemanticallySameJson(@"{""n"": 1}", customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void ShouldMatchJsonSchema_InvalidSchema_ReportsSchemaNotJson()
    {
        var actual = @"{""name"": ""John""}";
        var invalidSchema = "{invalid schema";

        var ex = Should.Throw<ShouldAssertException>(() => actual.ShouldMatchJsonSchema(invalidSchema));

        ex.Message.ShouldContain("Invalid JSON Schema");
        ex.Message.ShouldContain(actual);
    }

    [Fact]
    public void ShouldMatchJsonSchema_InvalidJson_ReportsInvalidJson()
    {
        var schema = @"{""type"": ""object""}";

        var ex = Should.Throw<ShouldAssertException>(() => "{invalid json}".ShouldMatchJsonSchema(schema));

        ex.Message.ShouldContain("Invalid JSON");
    }
}
