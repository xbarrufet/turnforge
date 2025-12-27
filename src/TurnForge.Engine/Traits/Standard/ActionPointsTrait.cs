using TurnForge.Engine.Traits.Interfaces;

namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Defines the Action Points configuration for an entity.
/// </summary>
public class ActionPointsTrait : BaseTrait
{
    public int MaxActionPoints { get; private set; }
    public int RegenerationRate { get; private set; }

    public ActionPointsTrait() { }

    public ActionPointsTrait(int maxActionPoints, int regenerationRate = 1)
    {
        MaxActionPoints = maxActionPoints;
        RegenerationRate = regenerationRate;
    }
}
