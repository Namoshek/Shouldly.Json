namespace Shouldly;

public class ShouldBeSemanticallySameJsonEnhancedTest
{
    [Fact]
    public void ShouldBeSemanticallySameJson_WithValueMismatch_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{""name"": ""John"", ""age"": 31}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/age': expected '31' but was '30'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithMissingProperty_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""name"": ""John""}";
        var expected = @"{""name"": ""John"", ""age"": 30}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON missing property at path '/age': expected property 'age' not found");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithExtraProperty_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{""name"": ""John""}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON extra property at path '/age': unexpected property 'age' found");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithArrayLengthMismatch_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"[1, 2]";
        var expected = @"[1, 2, 3]";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON array length mismatch at path '': expected 3 elements but was 2");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithArrayElementMismatch_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"[1, 2, 4]";
        var expected = @"[1, 2, 3]";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/2': expected '3' but was '4'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithNestedObjectDifference_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""person"": {""name"": ""John"", ""age"": 30}}";
        var expected = @"{""person"": {""name"": ""Jane"", ""age"": 30}}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/person/name': expected 'Jane' but was 'John'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithNestedArrayDifference_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""numbers"": [1, 2, 4]}";
        var expected = @"{""numbers"": [1, 2, 3]}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/numbers/2': expected '3' but was '4'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithTypeMismatch_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""value"": ""42""}";
        var expected = @"{""value"": 42}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON type mismatch at path '/value': expected number but was string");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithCustomMessage_ShouldCombineMessages()
    {
        var actual = @"{""name"": ""John""}";
        var expected = @"{""name"": ""Jane""}";
        var customMessage = "User data should match";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected, customMessage));
        
        exception.Message.ShouldContain("User data should match");
        exception.Message.ShouldContain("JSON value mismatch at path '/name': expected 'Jane' but was 'John'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithComplexNestedDifference_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""john@example.com""},
                        {""type"": ""phone"", ""value"": ""123-456-7890""}
                    ]
                }
            ]
        }";
        
        var expected = @"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""john@example.com""},
                        {""type"": ""phone"", ""value"": ""987-654-3210""}
                    ]
                }
            ]
        }";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/users/0/contacts/1/value': expected '987-654-3210' but was '123-456-7890'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithSpecialCharactersInPropertyNames_ShouldHandleCorrectly()
    {
        var actual = @"{""prop~with/special"": ""value1""}";
        var expected = @"{""prop~with/special"": ""value2""}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/prop~0with~1special': expected 'value2' but was 'value1'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithSuccessfulComparison_ShouldNotThrow()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{""age"": 30, ""name"": ""John""}";

        Should.NotThrow(() => actual.ShouldBeSemanticallySameJson(expected));
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithNullValues_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""value"": null}";
        var expected = @"{""value"": ""something""}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/value': expected 'something' but was 'null'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithBooleanDifference_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""active"": true}";
        var expected = @"{""active"": false}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/active': expected 'False' but was 'True'");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithInvalidJson_ShouldPreserveOriginalBehavior()
    {
        var actual = @"{invalid json}";
        var expected = @"{""name"": ""John""}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("invalid JSON provided");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithNullInputs_ShouldPreserveOriginalBehavior()
    {
        string? actual = null;
        var expected = @"{""name"": ""John""}";

        Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithBothNullInputs_ShouldNotThrow()
    {
        string? actual = null;
        string? expected = null;

        Should.NotThrow(() => actual.ShouldBeSemanticallySameJson(expected));
    }
}
