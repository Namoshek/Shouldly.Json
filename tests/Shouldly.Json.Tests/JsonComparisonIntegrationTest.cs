namespace Shouldly;

public class JsonComparisonIntegrationTest
{
    [Fact]
    public void CompleteWorkflow_SemanticEquality_ShouldProvideDetailedErrorMessages()
    {
        var scenarios = new[]
        {
            new
            {
                Name = "Value Mismatch",
                Actual = @"{""name"": ""John"", ""age"": 30}",
                Expected = @"{""name"": ""John"", ""age"": 31}",
                ExpectedError = "JSON value mismatch at path '/age': expected '31' but was '30'"
            },
            new
            {
                Name = "Missing Property",
                Actual = @"{""name"": ""John""}",
                Expected = @"{""name"": ""John"", ""age"": 30}",
                ExpectedError = "JSON missing property at path '/age': expected property 'age' not found"
            },
            new
            {
                Name = "Extra Property",
                Actual = @"{""name"": ""John"", ""age"": 30}",
                Expected = @"{""name"": ""John""}",
                ExpectedError = "JSON extra property at path '/age': unexpected property 'age' found"
            },
            new
            {
                Name = "Type Mismatch",
                Actual = @"{""value"": ""42""}",
                Expected = @"{""value"": 42}",
                ExpectedError = "JSON type mismatch at path '/value': expected number but was string"
            },
            new
            {
                Name = "Array Length Mismatch",
                Actual = @"[1, 2]",
                Expected = @"[1, 2, 3]",
                ExpectedError = "JSON array length mismatch at path '': expected 3 elements but was 2"
            },
            new
            {
                Name = "Array Element Mismatch",
                Actual = @"[1, 2, 4]",
                Expected = @"[1, 2, 3]",
                ExpectedError = "JSON value mismatch at path '/2': expected '3' but was '4'"
            },
            new
            {
                Name = "Nested Object Difference",
                Actual = @"{""person"": {""name"": ""John"", ""age"": 30}}",
                Expected = @"{""person"": {""name"": ""Jane"", ""age"": 30}}",
                ExpectedError = "JSON value mismatch at path '/person/name': expected 'Jane' but was 'John'"
            },
            new
            {
                Name = "Nested Array Difference",
                Actual = @"{""numbers"": [1, 2, 4]}",
                Expected = @"{""numbers"": [1, 2, 3]}",
                ExpectedError = "JSON value mismatch at path '/numbers/2': expected '3' but was '4'"
            }
        };

        foreach (var scenario in scenarios)
        {
            var exception = Should.Throw<ShouldAssertException>(() => scenario.Actual.ShouldBeSemanticallySameJson(scenario.Expected));
            
            exception.Message.ShouldContain(scenario.ExpectedError);
        }
    }

    [Fact]
    public void CompleteWorkflow_SubtreeMatching_ShouldProvideDetailedErrorMessages()
    {
        var scenarios = new[]
        {
            new
            {
                Name = "Missing Property in Superset",
                Actual = @"{""name"": ""John"", ""email"": ""john@example.com""}",
                Expected = @"{""name"": ""John"", ""age"": 30}",
                ExpectedError = "JSON missing property at path '/email': expected property 'email' not found"
            },
            new
            {
                Name = "Value Mismatch in Subtree",
                Actual = @"{""name"": ""John"", ""age"": 30}",
                Expected = @"{""name"": ""John"", ""age"": 31}",
                ExpectedError = "JSON value mismatch at path '/age': expected '31' but was '30'"
            },
            new
            {
                Name = "Array Length Mismatch in Subtree",
                Actual = @"{""items"": [1, 2]}",
                Expected = @"{""items"": [1, 2, 3]}",
                ExpectedError = "JSON array length mismatch at path '/items': expected 3 elements but was 2"
            }
        };

        foreach (var scenario in scenarios)
        {
            var exception = Should.Throw<ShouldAssertException>(() => scenario.Actual.ShouldBeJsonSubtreeOf(scenario.Expected));
            
            exception.Message.ShouldContain(scenario.ExpectedError);
        }
    }

    [Fact]
    public void CompleteWorkflow_WithCustomMessages_ShouldCombineCorrectly()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{""name"": ""John"", ""age"": 31}";
        var customMessage = "User data validation failed";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected, customMessage));
        
        exception.Message.ShouldContain(customMessage);
        exception.Message.ShouldContain("JSON value mismatch at path '/age': expected '31' but was '30'");
    }

    [Fact]
    public void CompleteWorkflow_WithComplexNestedStructures_ShouldProvideAccuratePaths()
    {
        var actual = @"{
            ""users"": [
                {
                    ""id"": 1,
                    ""profile"": {
                        ""personal"": {
                            ""name"": ""John"",
                            ""contacts"": [
                                {""type"": ""email"", ""value"": ""john@example.com""},
                                {""type"": ""phone"", ""value"": ""123-456-7890""}
                            ]
                        }
                    }
                }
            ]
        }";
        
        var expected = @"{
            ""users"": [
                {
                    ""id"": 1,
                    ""profile"": {
                        ""personal"": {
                            ""name"": ""John"",
                            ""contacts"": [
                                {""type"": ""email"", ""value"": ""john@example.com""},
                                {""type"": ""phone"", ""value"": ""987-654-3210""}
                            ]
                        }
                    }
                }
            ]
        }";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/users/0/profile/personal/contacts/1/value': expected '987-654-3210' but was '123-456-7890'");
    }

    [Fact]
    public void CompleteWorkflow_WithSpecialCharactersInPaths_ShouldEscapeCorrectly()
    {
        var actual = @"{""prop~with/special"": ""value1""}";
        var expected = @"{""prop~with/special"": ""value2""}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/prop~0with~1special': expected 'value2' but was 'value1'");
    }

    [Fact]
    public void CompleteWorkflow_BackwardCompatibility_ShouldPreserveExistingBehavior()
    {
        // Test that null inputs still work as before
        string? actual = null;
        var expected = @"{""name"": ""John""}";

        Should.Throw<ShouldAssertException>(() => actual.ShouldBeSemanticallySameJson(expected));

        // Test that both null inputs don't throw
        string? actualNull = null;
        string? expectedNull = null;
        Should.NotThrow(() => actualNull.ShouldBeSemanticallySameJson(expectedNull));

        // Test that invalid JSON still throws with appropriate message
        var invalidJson = @"{invalid json}";
        var validJson = @"{""name"": ""John""}";
        
        var exception = Should.Throw<ShouldAssertException>(() => invalidJson.ShouldBeSemanticallySameJson(validJson));
        
        exception.Message.ShouldContain("invalid JSON provided");
    }

    [Fact]
    public void CompleteWorkflow_AllAssertionMethods_ShouldUseEnhancedErrorMessages()
    {
        // Test ShouldBeSemanticallySameJson
        var exception1 = Should.Throw<ShouldAssertException>(() => @"{""a"": 1}".ShouldBeSemanticallySameJson(@"{""a"": 2}"));
        exception1.Message.ShouldContain("JSON value mismatch at path '/a'");

        // Test ShouldBeJsonSubtreeOf
        var exception2 = Should.Throw<ShouldAssertException>(() => @"{""a"": 1, ""b"": 2}".ShouldBeJsonSubtreeOf(@"{""a"": 1}"));
        exception2.Message.ShouldContain("JSON missing property at path '/b'");

        // Test ShouldHaveJsonSubtree
        var exception3 = Should.Throw<ShouldAssertException>(() => @"{""a"": 1}".ShouldHaveJsonSubtree(@"{""a"": 1, ""b"": 2}"));
        exception3.Message.ShouldContain("JSON missing property at path '/b'");
    }
}
