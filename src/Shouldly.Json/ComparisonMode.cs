namespace Shouldly;

/// <summary>
/// Defines the different modes of JSON comparison.
/// </summary>
public enum ComparisonMode
{
    /// <summary>
    /// Semantic equality mode where both JSON structures must be exactly equivalent.
    /// Property order in objects doesn't matter, but all properties must match.
    /// Array order and length must match exactly.
    /// </summary>
    SemanticEquality,

    /// <summary>
    /// Subtree matching mode where the actual JSON must be a subset of the expected JSON.
    /// All properties in the actual JSON must exist in the expected JSON with matching values,
    /// but the expected JSON can have additional properties.
    /// Arrays must match exactly in length and content.
    /// </summary>
    SubtreeMatching,
}
