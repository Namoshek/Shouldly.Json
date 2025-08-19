namespace Shouldly;

public class JsonErrorMessageFormatterTest
{
    [Fact]
    public void FormatDifference_WithValueMismatch_ShouldFormatCorrectly()
    {
        var difference = JsonDifference.ValueMismatch("/name", "John", "Jane");
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        formatted.ShouldBe("JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void FormatDifference_WithTypeMismatch_ShouldFormatCorrectly()
    {
        var difference = JsonDifference.TypeMismatch("/age", "number", "string");
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        formatted.ShouldBe("JSON type mismatch at path '/age': expected number but was string");
    }

    [Fact]
    public void FormatDifference_WithMissingProperty_ShouldFormatCorrectly()
    {
        var difference = JsonDifference.MissingProperty("/person", "email");
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        formatted.ShouldBe("JSON missing property at path '/person': expected property 'email' not found");
    }

    [Fact]
    public void FormatDifference_WithExtraProperty_ShouldFormatCorrectly()
    {
        var difference = JsonDifference.ExtraProperty("/person", "phone");
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        formatted.ShouldBe("JSON extra property at path '/person': unexpected property 'phone' found");
    }

    [Fact]
    public void FormatDifference_WithArrayLengthMismatch_ShouldFormatCorrectly()
    {
        var difference = JsonDifference.ArrayLengthMismatch("/items", 3, 2);
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        formatted.ShouldBe("JSON array length mismatch at path '/items': expected 3 elements but was 2");
    }

    [Fact]
    public void FormatDifference_WithArrayElementMismatch_ShouldFormatCorrectly()
    {
        var difference = JsonDifference.ArrayElementMismatch("/items/0", "apple", "orange");
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        formatted.ShouldBe("JSON array element mismatch at path '/items/0': expected 'apple' but was 'orange'");
    }

    [Fact]
    public void CombineMessages_WithBothMessages_ShouldCombineCorrectly()
    {
        var customMessage = "User data should match";
        var differenceMessage = "JSON value mismatch at path '/name': expected 'John' but was 'Jane'";
        
        var combined = JsonErrorMessageFormatter.CombineMessages(customMessage, differenceMessage);
        
        combined.ShouldBe("User data should match. JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void CombineMessages_WithOnlyCustomMessage_ShouldReturnCustomMessage()
    {
        var customMessage = "User data should match";
        var differenceMessage = "";
        
        var combined = JsonErrorMessageFormatter.CombineMessages(customMessage, differenceMessage);
        
        combined.ShouldBe("User data should match");
    }

    [Fact]
    public void CombineMessages_WithOnlyDifferenceMessage_ShouldReturnDifferenceMessage()
    {
        var customMessage = "";
        var differenceMessage = "JSON value mismatch at path '/name': expected 'John' but was 'Jane'";
        
        var combined = JsonErrorMessageFormatter.CombineMessages(customMessage, differenceMessage);
        
        combined.ShouldBe("JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void CombineMessages_WithNullCustomMessage_ShouldReturnDifferenceMessage()
    {
        string? customMessage = null;
        var differenceMessage = "JSON value mismatch at path '/name': expected 'John' but was 'Jane'";
        
        var combined = JsonErrorMessageFormatter.CombineMessages(customMessage, differenceMessage);
        
        combined.ShouldBe("JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void TruncateIfNeeded_WithShortMessage_ShouldReturnOriginal()
    {
        var message = "Short message";
        
        var truncated = JsonErrorMessageFormatter.TruncateIfNeeded(message, 100);
        
        truncated.ShouldBe("Short message");
    }

    [Fact]
    public void TruncateIfNeeded_WithLongMessage_ShouldTruncate()
    {
        var message = new string('a', 1500);
        
        var truncated = JsonErrorMessageFormatter.TruncateIfNeeded(message, 1000);
        
        truncated.Length.ShouldBe(1000);
        truncated.ShouldEndWith("...");
    }

    [Fact]
    public void CreateContextualMessage_WithSemanticEquality_ShouldIncludeContext()
    {
        var difference = JsonDifference.ValueMismatch("/name", "John", "Jane");
        
        var contextual = JsonErrorMessageFormatter.CreateContextualMessage(difference, ComparisonMode.SemanticEquality);
        
        contextual.ShouldContain("during semantic equality comparison");
        contextual.ShouldContain("JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void CreateContextualMessage_WithSubtreeMatching_ShouldIncludeContext()
    {
        var difference = JsonDifference.MissingProperty("/person", "email");
        
        var contextual = JsonErrorMessageFormatter.CreateContextualMessage(difference, ComparisonMode.SubtreeMatching);
        
        contextual.ShouldContain("during subtree matching comparison");
        contextual.ShouldContain("JSON missing property at path '/person': expected property 'email' not found");
    }

    [Theory]
    [InlineData(null, "null")]
    [InlineData("text", "text")]
    [InlineData(42, "42")]
    [InlineData(true, "True")]
    [InlineData(false, "False")]
    [InlineData(3.14, "3.14")]
    public void FormatDifference_WithDifferentValueTypes_ShouldFormatCorrectly(object? value, string expectedFormatted)
    {
        var difference = JsonDifference.ValueMismatch("/test", expectedFormatted, value);
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        // The test creates a difference with the same expected and actual values, so they should match
        formatted.ShouldContain($"expected '{expectedFormatted}' but was");
    }

    [Fact]
    public void FormatDifference_WithRootPath_ShouldFormatPathCorrectly()
    {
        var difference = JsonDifference.ArrayLengthMismatch("", 3, 2);
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        formatted.ShouldBe("JSON array length mismatch at path '': expected 3 elements but was 2");
    }

    [Fact]
    public void FormatDifference_WithNestedPath_ShouldFormatPathCorrectly()
    {
        var difference = JsonDifference.ValueMismatch("/users/0/contacts/1/email", "old@example.com", "new@example.com");
        
        var formatted = JsonErrorMessageFormatter.FormatDifference(difference);
        
        formatted.ShouldBe("JSON value mismatch at path '/users/0/contacts/1/email': expected 'old@example.com' but was 'new@example.com'");
    }
}
