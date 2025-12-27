namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Defines the immutable identity metadata of an entity.
/// Replaces generic Category/Name properties.
/// </summary>
public class IdentityTrait : BaseTrait
{
    public string Name { get; }
    public string Category { get; }

    public IdentityTrait(string name, string category)
    {
        Name = name;
        Category = category;
    }

    // Default for deserialization or empty init
    public IdentityTrait() : this(string.Empty, string.Empty) { }
}
