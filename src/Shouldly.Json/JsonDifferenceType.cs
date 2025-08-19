namespace Shouldly;

/// <summary>
/// Represents the different types of differences that can be found during JSON comparison.
/// </summary>
internal enum JsonDifferenceType
{
    /// <summary>
    /// A value mismatch where the same property has different values.
    /// </summary>
    ValueMismatch,

    /// <summary>
    /// A type mismatch where the same property has different JSON types.
    /// </summary>
    TypeMismatch,

    /// <summary>
    /// A missing property that exists in the expected JSON but not in the actual JSON.
    /// </summary>
    MissingProperty,

    /// <summary>
    /// An extra property that exists in the actual JSON but not in the expected JSON.
    /// </summary>
    ExtraProperty,

    /// <summary>
    /// An array length mismatch where arrays have different numbers of elements.
    /// </summary>
    ArrayLengthMismatch,

    /// <summary>
    /// An array element mismatch where arrays have different values at the same index.
    /// </summary>
    ArrayElementMismatch,
}
