using TurnForge.Engine.Traits.Standard;
using TurnForge.Engine.Traits;

namespace BarelyAlive.Rules.Core.Domain.Traits;

public class SpawnOrderTrait : BaseTrait
{
    public int Order { get; private set; } = 1;

    public SpawnOrderTrait() { }
    
    public SpawnOrderTrait(int order)
    {
        Order = order;
    }
}
