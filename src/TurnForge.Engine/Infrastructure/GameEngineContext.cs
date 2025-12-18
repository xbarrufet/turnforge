using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Registration;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn.Interfaces;

namespace TurnForge.Engine.Infrastructure;

public readonly struct GameEngineContext(
    IGameRepository gameRepository,
    IDefinitionRegistry<PropTypeId, PropDefinition> propDefinitions,
    IDefinitionRegistry<UnitTypeId, UnitDefinition> unitDefinitions,
    IDefinitionRegistry<NpcTypeId, NpcDefinition> npcDefinitions,
    IPropSpawnStrategy propSpawnStrategy,
    IUnitSpawnStrategy unitSpawnStrategy)
{
    public IGameRepository GameRepository { get; init; } = gameRepository;

    public IDefinitionRegistry<PropTypeId, PropDefinition> PropDefinitions { get; init; } = propDefinitions;
    public IDefinitionRegistry<UnitTypeId, UnitDefinition> UnitDefinitions { get; init; } = unitDefinitions;
    public IDefinitionRegistry<NpcTypeId, NpcDefinition> NpcDefinitions { get; init; } = npcDefinitions;

    public IPropSpawnStrategy PropSpawnStrategy { get; init; } = propSpawnStrategy;
    public IUnitSpawnStrategy UnitSpawnStrategy { get; init; } = unitSpawnStrategy;
}
