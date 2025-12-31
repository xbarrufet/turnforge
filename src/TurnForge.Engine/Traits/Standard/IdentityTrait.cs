namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Defines the immutable identity metadata of an entity.
/// Replaces generic Category/Name properties.
/// </summary>
public class IdentityTrait(string category) : BaseDataTrait
{
    public string Category { get; } = category;

    public IdentityTrait():this("Common") {
    }
        

}
