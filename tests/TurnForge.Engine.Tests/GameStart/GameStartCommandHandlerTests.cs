using System.Collections.Generic;using NUnit.Framework;using TurnForge.Engine.Commands.GameStart;using TurnForge.Engine.Repositories.Interfaces;using TurnForge.Engine.Entities;using TurnForge.Engine.Entities.Actors;using TurnForge.Engine.ValueObjects;using TurnForge.Engine.Entities.Actors.Definitions;using TurnForge.Engine.Strategies.Spawn.Interfaces;using TurnForge.Engine.Strategies.Spawn;using TurnForge.Engine.Infrastructure.Appliers;using System.Linq;
using TurnForge.Engine.Descriptors;

namespace TurnForge.Engine.Tests.GameStart
{
    public class GameStartCommandHandlerTests
    {
        private class InMemoryRepo : IGameRepository
        {
            public TurnForge.Engine.Entities.GameState State = TurnForge.Engine.Entities.GameState.Empty();
            public TurnForge.Engine.Entities.GameState Load() => State;
            public void SaveGameState(TurnForge.Engine.Entities.GameState state) => State = state;

            // Implement other interface members as no-op for tests
            public void SaveGame(TurnForge.Engine.Core.Game game) => throw new System.NotImplementedException();
            public TurnForge.Engine.Core.Game LoadGame(TurnForge.Engine.ValueObjects.GameId gameId) => throw new System.NotImplementedException();
            public TurnForge.Engine.Core.Game? GetCurrent() => null;
        }

        private class TestEffectSink : TurnForge.Engine.Core.Interfaces.IEffectSink
        {
            public readonly List<TurnForge.Engine.Core.Interfaces.IGameEffect> Emitted = new();
            public void Emit(TurnForge.Engine.Core.Interfaces.IGameEffect effect) => Emitted.Add(effect);
        }

        private class TestActorFactory : TurnForge.Engine.Entities.Actors.Interfaces.IActorFactory
        {
            public List<Unit> BuiltUnits = new();
            public Unit BuildUnit(TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId typeId, Position position, IReadOnlyList<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>? behaviours = null)
            {
                var id = new ActorId(System.Guid.NewGuid());
                var def = new UnitDefinition(typeId, 10, 3, 2, behaviours??new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
                var u = new Unit(id, position, def, behaviours?.ToList());
                BuiltUnits.Add(u);
                return u;
            }

            public Prop BuildProp(TurnForge.Engine.Entities.Actors.Definitions.PropTypeId typeId, Position position, IReadOnlyList<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>? behaviours = null)
            {
                var id = new ActorId(System.Guid.NewGuid());
                var def = new TurnForge.Engine.Entities.Actors.Definitions.PropDefinition(typeId, 0, 0, behaviours??new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>(), 10);
                return new Prop(id, position, def, null, behaviours?.ToList());
            }

            public Npc BuildNpc(TurnForge.Engine.Entities.Actors.Definitions.NpcTypeId typeId, Position position, IReadOnlyList<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>? behaviours = null)
            {
                var id = new ActorId(System.Guid.NewGuid());
                var def = new TurnForge.Engine.Entities.Actors.Definitions.NpcDefinition(typeId, 10, 3, 2, behaviours??new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
                return new Npc(id, position, def, behaviours?.ToList());
            }
        }

        private class StubStrategy : IUnitSpawnStrategy
        {
            private readonly IReadOnlyList<UnitSpawnDecision> _decisions;
            public StubStrategy(IReadOnlyList<UnitSpawnDecision> decisions) => _decisions = decisions;
            public IReadOnlyList<UnitSpawnDecision> Decide(UnitSpawnContext context) => _decisions;
        }

        [Test]
        public void GameStartHandler_CreatesUnits_FromStrategyPositions()
        {
            var repo = new InMemoryRepo();
            var effectSink = new TestEffectSink();
            var factory = new TestActorFactory();

            var unit1 = new UnitDescriptor(new TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId("u1"), null);
            var unit2 = new UnitDescriptor(new TurnForge.Engine.Entities.Actors.Definitions.UnitTypeId("u2"), null);

            var decision1 = new UnitSpawnDecision(unit1.TypeId, new Position(5,5), new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
            var decision2 = new UnitSpawnDecision(unit2.TypeId, new Position(6,6), new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
            var strategy = new StubStrategy(new []{decision1, decision2});

            var handler = new TurnForge.Engine.Commands.GameStart.GameStartCommandHandler(repo, strategy, factory, effectSink);

            var command = new GameStartCommand(new List<UnitDescriptor>{unit1, unit2});
            var result = handler.Handle(command);

            NUnit.Framework.Assert.That(result.Success, Is.True);
            NUnit.Framework.Assert.That(repo.State.Units.Count, Is.EqualTo(2));

            var units = repo.State.Units.Values.ToList();
            NUnit.Framework.Assert.That(units.Any(u => u.Position.Equals(new Position(5,5))), Is.True);
            NUnit.Framework.Assert.That(units.Any(u => u.Position.Equals(new Position(6,6))), Is.True);
        }
    }
}
