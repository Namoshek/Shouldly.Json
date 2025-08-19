namespace Shouldly;

public class JsonDifferenceTest
{
    [Fact]
    public void ValueMismatch_ShouldCreateCorrectDifference()
    {
        var difference = JsonDifference.ValueMismatch("/name", "John", "Jane");

        difference.Path.ShouldBe("/name");
        difference.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        difference.ExpectedValue.ShouldBe("John");
        difference.ActualValue.ShouldBe("Jane");
        difference.Description.ShouldBe("JSON value mismatch at path '/name': expected 'John' but was 'Jane'");
    }

    [Fact]
    public void TypeMismatch_ShouldCreateCorrectDifference()
    {
        var difference = JsonDifference.TypeMismatch("/age", "number", "string");

        difference.Path.ShouldBe("/age");
        difference.Type.ShouldBe(JsonDifferenceType.TypeMismatch);
        difference.ExpectedType.ShouldBe("number");
        difference.ActualType.ShouldBe("string");
        difference.Description.ShouldBe("JSON type mismatch at path '/age': expected number but was string");
    }

    [Fact]
    public void MissingProperty_ShouldCreateCorrectDifference()
    {
        var difference = JsonDifference.MissingProperty("/person", "email");

        difference.Path.ShouldBe("/person");
        difference.Type.ShouldBe(JsonDifferenceType.MissingProperty);
        difference.ExpectedValue.ShouldBe("email");
        difference.Description.ShouldBe("JSON missing property at path '/person': expected property 'email' not found");
    }

    [Fact]
    public void ExtraProperty_ShouldCreateCorrectDifference()
    {
        var difference = JsonDifference.ExtraProperty("/person", "phone");

        difference.Path.ShouldBe("/person");
        difference.Type.ShouldBe(JsonDifferenceType.ExtraProperty);
        difference.ActualValue.ShouldBe("phone");
        difference.Description.ShouldBe("JSON extra property at path '/person': unexpected property 'phone' found");
    }

    [Fact]
    public void ArrayLengthMismatch_ShouldCreateCorrectDifference()
    {
        var difference = JsonDifference.ArrayLengthMismatch("/items", 3, 2);

        difference.Path.ShouldBe("/items");
        difference.Type.ShouldBe(JsonDifferenceType.ArrayLengthMismatch);
        difference.ExpectedValue.ShouldBe(3);
        difference.ActualValue.ShouldBe(2);
        difference.Description.ShouldBe("JSON array length mismatch at path '/items': expected 3 elements but was 2");
    }

    [Fact]
    public void ArrayElementMismatch_ShouldCreateCorrectDifference()
    {
        var difference = JsonDifference.ArrayElementMismatch("/items/0", "apple", "orange");

        difference.Path.ShouldBe("/items/0");
        difference.Type.ShouldBe(JsonDifferenceType.ArrayElementMismatch);
        difference.ExpectedValue.ShouldBe("apple");
        difference.ActualValue.ShouldBe("orange");
        difference.Description.ShouldBe("JSON array element mismatch at path '/items/0': expected 'apple' but was 'orange'");
    }

    [Fact]
    public void ValueMismatch_WithNullValues_ShouldFormatCorrectly()
    {
        var difference = JsonDifference.ValueMismatch("/value", null, "something");

        difference.Description.ShouldBe("JSON value mismatch at path '/value': expected 'null' but was 'something'");
    }

    [Fact]
    public void ValueMismatch_WithNumericValues_ShouldFormatCorrectly()
    {
        var difference = JsonDifference.ValueMismatch("/count", 42, 43);

        difference.Description.ShouldBe("JSON value mismatch at path '/count': expected '42' but was '43'");
    }
}
