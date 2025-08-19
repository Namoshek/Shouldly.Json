namespace Shouldly;

public class ShouldHaveJsonSubtreeEnhancedTest
{
    [Fact]
    public void ShouldHaveJsonSubtree_WithMissingPropertyInSuperset_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{""name"": ""John"", ""email"": ""john@example.com""}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldHaveJsonSubtree(expected));
        
        exception.Message.ShouldContain("JSON missing property at path '/email': expected property 'email' not found");
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithValueMismatch_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""name"": ""John"", ""age"": 31}";
        var expected = @"{""name"": ""John"", ""age"": 30}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldHaveJsonSubtree(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/age': expected '31' but was '30'");
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithArrayLengthMismatch_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""items"": [1, 2, 3]}";
        var expected = @"{""items"": [1, 2]}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldHaveJsonSubtree(expected));
        
        exception.Message.ShouldContain("JSON array length mismatch at path '/items': expected 3 elements but was 2");
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithNestedObjectDifference_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""person"": {""name"": ""John"", ""age"": 30}}";
        var expected = @"{""person"": {""name"": ""John"", ""contact"": {""email"": ""john@example.com""}}}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldHaveJsonSubtree(expected));
        
        exception.Message.ShouldContain("JSON missing property at path '/person/contact': expected property 'contact' not found");
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithSuccessfulSubtree_ShouldNotThrow()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{""name"": ""John""}";

        Should.NotThrow(() => actual.ShouldHaveJsonSubtree(expected));
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithCustomMessage_ShouldCombineMessages()
    {
        var actual = @"{""name"": ""John""}";
        var expected = @"{""name"": ""John"", ""extra"": ""value""}";
        var customMessage = "JSON should contain expected subset";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldHaveJsonSubtree(expected, customMessage));
        
        exception.Message.ShouldContain("JSON should contain expected subset");
        exception.Message.ShouldContain("JSON missing property at path '/extra': expected property 'extra' not found");
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithArrayOfObjectsSubset_ShouldNotThrow()
    {
        var actual = @"[{""name"": ""John"", ""age"": 30}]";
        var expected = @"[{""name"": ""John""}]";

        Should.NotThrow(() => actual.ShouldHaveJsonSubtree(expected));
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithArrayOfObjectsExtraProperty_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"[{""name"": ""John"", ""age"": 30}]";
        var expected = @"[{""name"": ""John"", ""email"": ""john@example.com""}]";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldHaveJsonSubtree(expected));
        
        exception.Message.ShouldContain("JSON missing property at path '/0/email': expected property 'email' not found");
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithComplexNestedStructure_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""john@example.com""}
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
                        {""type"": ""phone"", ""value"": ""123-456-7890""}
                    ]
                }
            ]
        }";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldHaveJsonSubtree(expected));
        
        exception.Message.ShouldContain("JSON array length mismatch at path '/users/0/contacts': expected 1 elements but was 2");
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithEmptyObjectSubset_ShouldNotThrow()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{}";

        Should.NotThrow(() => actual.ShouldHaveJsonSubtree(expected));
    }

    [Fact]
    public void ShouldHaveJsonSubtree_WithEmptyArraySubset_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"[1, 2, 3]";
        var expected = @"[]";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldHaveJsonSubtree(expected));
        
        exception.Message.ShouldContain("JSON array length mismatch at path '': expected 3 elements but was 0");
    }
}
