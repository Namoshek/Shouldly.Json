namespace Shouldly;

public class ShouldBeJsonSubtreeOfEnhancedTest
{
    [Fact]
    public void ShouldBeJsonSubtreeOf_WithMissingPropertyInSuperset_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""name"": ""John"", ""email"": ""john@example.com""}";
        var expected = @"{""name"": ""John"", ""age"": 30}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeJsonSubtreeOf(expected));
        
        exception.Message.ShouldContain("JSON missing property at path '/email': expected property 'email' not found");
    }

    [Fact]
    public void ShouldBeJsonSubtreeOf_WithValueMismatch_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""name"": ""John"", ""age"": 30}";
        var expected = @"{""name"": ""John"", ""age"": 31}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeJsonSubtreeOf(expected));
        
        exception.Message.ShouldContain("JSON value mismatch at path '/age': expected '31' but was '30'");
    }

    [Fact]
    public void ShouldBeJsonSubtreeOf_WithArrayLengthMismatch_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""items"": [1, 2]}";
        var expected = @"{""items"": [1, 2, 3]}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeJsonSubtreeOf(expected));
        
        exception.Message.ShouldContain("JSON array length mismatch at path '/items': expected 3 elements but was 2");
    }

    [Fact]
    public void ShouldBeJsonSubtreeOf_WithNestedObjectDifference_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"{""person"": {""name"": ""John"", ""contact"": {""email"": ""john@example.com""}}}";
        var expected = @"{""person"": {""name"": ""John"", ""age"": 30}}";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeJsonSubtreeOf(expected));
        
        exception.Message.ShouldContain("JSON missing property at path '/person/contact': expected property 'contact' not found");
    }

    [Fact]
    public void ShouldBeJsonSubtreeOf_WithSuccessfulSubtree_ShouldNotThrow()
    {
        var actual = @"{""name"": ""John""}";
        var expected = @"{""name"": ""John"", ""age"": 30}";

        Should.NotThrow(() => actual.ShouldBeJsonSubtreeOf(expected));
    }

    [Fact]
    public void ShouldBeJsonSubtreeOf_WithCustomMessage_ShouldCombineMessages()
    {
        var actual = @"{""name"": ""John"", ""extra"": ""value""}";
        var expected = @"{""name"": ""John""}";
        var customMessage = "User subset should match";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeJsonSubtreeOf(expected, customMessage));
        
        exception.Message.ShouldContain("User subset should match");
        exception.Message.ShouldContain("JSON missing property at path '/extra': expected property 'extra' not found");
    }

    [Fact]
    public void ShouldBeJsonSubtreeOf_WithArrayOfObjectsSubset_ShouldNotThrow()
    {
        var actual = @"[{""name"": ""John""}]";
        var expected = @"[{""name"": ""John"", ""age"": 30}]";

        Should.NotThrow(() => actual.ShouldBeJsonSubtreeOf(expected));
    }

    [Fact]
    public void ShouldBeJsonSubtreeOf_WithArrayOfObjectsExtraProperty_ShouldProvideDetailedErrorMessage()
    {
        var actual = @"[{""name"": ""John"", ""email"": ""john@example.com""}]";
        var expected = @"[{""name"": ""John"", ""age"": 30}]";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeJsonSubtreeOf(expected));
        
        exception.Message.ShouldContain("JSON missing property at path '/0/email': expected property 'email' not found");
    }

    [Fact]
    public void ShouldBeJsonSubtreeOf_WithComplexNestedStructure_ShouldProvideDetailedErrorMessage()
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
                        {""type"": ""email"", ""value"": ""john@example.com""}
                    ]
                }
            ]
        }";

        var exception = Should.Throw<ShouldAssertException>(() => actual.ShouldBeJsonSubtreeOf(expected));
        
        exception.Message.ShouldContain("JSON array length mismatch at path '/users/0/contacts': expected 1 elements but was 2");
    }
}
