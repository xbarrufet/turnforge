using TurnForge.Engine.Traits.Standard;
using BarelyAlive.Rules.Core.Domain.Entities;
using TurnForge.Engine.Traits;

namespace BarelyAlive.Rules.Core.Domain.Traits;

public class ColorTrait : BaseTrait
{
    public Color Color { get; private set; }

    public ColorTrait() { }
    
    public ColorTrait(Color color)
    {
        Color = color;
    }
}
