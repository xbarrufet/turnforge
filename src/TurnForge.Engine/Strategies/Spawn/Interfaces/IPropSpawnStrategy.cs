
namespace TurnForge.Engine.Strategies.Spawn.Interfaces;

public interface IPropSpawnStrategy
{
    IReadOnlyList<PropSpawnDecision> Decide(
       PropSpawnContext context
    );
}