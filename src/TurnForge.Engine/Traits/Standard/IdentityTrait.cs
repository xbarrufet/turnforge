namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Defines the immutable identity metadata of an entity.
/// Replaces generic Category/Name properties.
/// </summary>
public class IdentityTrait(string category) : BaseTrait
{
    public string Category { get; } = category;

    public class Builder
    {
        private string _category = "Common";
        public Builder Category(string category) { _category = category; return this; }
        public IdentityTrait Build() => new(_category);
    }
}
