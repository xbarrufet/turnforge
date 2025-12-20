using BarelyAlive.Rules.Events.Interfaces;

namespace BarelyAlive.Rules.Infrastructure;

public interface IBarelyAliveEffectsSink
{

    public void Emit(IBarelyAliveEffect effect);
}
