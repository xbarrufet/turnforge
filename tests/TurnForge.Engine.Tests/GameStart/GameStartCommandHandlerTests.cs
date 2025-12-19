using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Commands.GameStart;
using TurnForge.Engine.Descriptors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Infrastructure.Appliers;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.GameStart
{
    public class GameStartCommandHandlerTests
    {
        private class InMemoryRepo : IGameRepository
        {
            public TurnForge.Engine.Entities.GameState State = TurnForge.Engine.Entities.GameState.Empty();
            public TurnForge.Engine.Entities.GameState LoadGameState() => State;
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
            public void Subscribe(System.Action<TurnForge.Engine.Core.Interfaces.IGameEffect> action) { } // No-op
        }

        private class TestActorFactory : TurnForge.Engine.Entities.Actors.Interfaces.IActorFactory
        {
            public List<Agent> BuiltAgents = new();
            public Agent BuildAgent(TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId typeId, Position position, IReadOnlyList<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>? behaviours = null)
            {
                var id = new ActorId(System.Guid.NewGuid());
                var def = new AgentDefinition(typeId, 10, 3, 2, behaviours ?? new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
                var u = new Agent(id, position, def, behaviours?.ToList());
                BuiltAgents.Add(u);
                return u;
            }

            public Prop BuildProp(TurnForge.Engine.Entities.Actors.Definitions.PropTypeId typeId, Position position, IReadOnlyList<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>? behaviours = null)
            {
                var id = new ActorId(System.Guid.NewGuid());
                var def = new TurnForge.Engine.Entities.Actors.Definitions.PropDefinition(typeId, 0, 0, behaviours ?? new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>(), 10);
                return new Prop(id, position, def, null, behaviours?.ToList());
            }
        }

        private class StubStrategy : IAgentSpawnStrategy
        {
            private readonly IReadOnlyList<AgentSpawnDecision> _decisions;
            public StubStrategy(IReadOnlyList<AgentSpawnDecision> decisions) => _decisions = decisions;
            public IReadOnlyList<AgentSpawnDecision> Decide(AgentSpawnContext context) => _decisions;
        }

        [Test]
        public void GameStartHandler_CreatesAgents_FromStrategyPositions()
        {
            var repo = new InMemoryRepo();
            var effectSink = new TestEffectSink();
            var factory = new TestActorFactory();

            var unit1 = new AgentDescriptor(new TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId("u1"), null);
            var unit2 = new AgentDescriptor(new TurnForge.Engine.Entities.Actors.Definitions.AgentTypeId("u2"), null);

            var decision1 = new AgentSpawnDecision(unit1.TypeId, new Position(5, 5), new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
            var decision2 = new AgentSpawnDecision(unit2.TypeId, new Position(6, 6), new List<TurnForge.Engine.Entities.Actors.Interfaces.IActorBehaviour>());
            var strategy = new StubStrategy(new[] { decision1, decision2 });

            var handler = new TurnForge.Engine.Commands.GameStart.GameStartCommandHandler(repo, strategy, factory, effectSink);

            var command = new GameStartCommand(new List<AgentDescriptor> { unit1, unit2 });
            var result = handler.Handle(command);

            NUnit.Framework.Assert.That(result.Success, Is.True);
            NUnit.Framework.Assert.That(repo.State.GetAgents().Count, Is.EqualTo(2));

            var units = repo.State.GetAgents();
            NUnit.Framework.Assert.That(units.Any(u => u.Position.Equals(new Position(5, 5))), Is.True);
            NUnit.Framework.Assert.That(units.Any(u => u.Position.Equals(new Position(6, 6))), Is.True);
        }
    }
}
