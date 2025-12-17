using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Strategies.Spawn;


    public sealed record PropSpawnDecision(
        PropDescriptor Descriptor,
        Position Position
    );
