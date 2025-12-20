using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Commands.Game;
using TurnForge.Engine.Commands.Game.Descriptors;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Descriptors;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Definitions;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Appliers;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Descriptors;
using TurnForge.Engine.Infrastructure.Factories.Interfaces;
using TurnForge.Engine.Repositories.Interfaces;
using TurnForge.Engine.Spatial;
using TurnForge.Engine.Strategies.Spawn;
using TurnForge.Engine.Strategies.Spawn.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Game
{
    public class InitGameCommandHandlerTests
    {
        private class InMemoryRepo : IGameRepository
        {
            public TurnForge.Engine.Entities.GameState State = TurnForge.Engine.Entities.GameState.Empty();
            public TurnForge.Engine.Entities.GameState LoadGameState() => State;
            public void SaveGameState(TurnForge.Engine.Entities.GameState state) => State = state;
            public void SaveGame(TurnForge.Engine.Core.Game game) { }
            public TurnForge.Engine.Core.Game LoadGame(GameId gameId) => throw new System.NotImplementedException();
            public TurnForge.Engine.Core.Game? GetCurrent() => null;
        }

        private class TestEffectSink : IEffectSink
        {
            public readonly List<IGameEffect> Emitted = new();
            public void Emit(IGameEffect effect) => Emitted.Add(effect);
            public void Subscribe(System.Action<IGameEffect> action) { }
        }

        private class TestActorFactory : IActorFactory
        {
            public List<Agent> BuiltAgents = new();
            public Agent BuildAgent(AgentTypeId typeId, IEnumerable<ActorBehaviour>? behaviours = null)
            {
                var id = EntityId.New();
                var behavioursList = behaviours?.Cast<IActorBehaviour>().ToList() ?? new List<IActorBehaviour>();
                var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(behaviours?.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>() ?? Enumerable.Empty<TurnForge.Engine.Entities.Components.BaseBehaviour>());

                var def = new AgentDefinition(typeId,
                    new TurnForge.Engine.Entities.Components.Definitions.PositionComponentDefinition(Position.Empty),
                    new TurnForge.Engine.Entities.Components.Definitions.HealhtComponentDefinition(10),
                    new TurnForge.Engine.Entities.Components.Definitions.MovementComponentDefinition(3),
                    behavioursList);
                var u = new Agent(id, def, new TurnForge.Engine.Entities.Components.PositionComponent(def.PositionComponentDefinition), component);
                BuiltAgents.Add(u);
                return u;
            }

            public Prop BuildProp(PropTypeId typeId, IEnumerable<ActorBehaviour>? behaviours = null)
            {
                var id = EntityId.New();
                var behavioursList = behaviours?.Cast<IActorBehaviour>().ToList() ?? new List<IActorBehaviour>();
                var component = new TurnForge.Engine.Entities.Components.BehaviourComponent(behaviours?.Cast<TurnForge.Engine.Entities.Components.BaseBehaviour>() ?? Enumerable.Empty<TurnForge.Engine.Entities.Components.BaseBehaviour>());

                var def = new PropDefinition(typeId,
                    new TurnForge.Engine.Entities.Components.Definitions.PositionComponentDefinition(Position.Empty),
                    new TurnForge.Engine.Entities.Components.Definitions.HealhtComponentDefinition(10),
                    behavioursList);
                return new Prop(id, def, new TurnForge.Engine.Entities.Components.PositionComponent(def.PositionComponentDefinition), component);
            }
        }

        private class StubAgentStrategy(IReadOnlyList<AgentSpawnDecision> decisions) : IAgentSpawnStrategy
        {
            public IReadOnlyList<AgentSpawnDecision> Decide(AgentSpawnContext context) => decisions;
        }

        private class StubPropStrategy(IReadOnlyList<PropSpawnDecision> decisions) : IPropSpawnStrategy
        {
            public IReadOnlyList<PropSpawnDecision> Decide(PropSpawnContext context) => decisions;
        }

        private class StubGameFactory : IGameFactory
        {
            public TurnForge.Engine.Core.Game Build(GameBoard board) => new TurnForge.Engine.Core.Game(new GameId(System.Guid.NewGuid()), board);
        }

        [Test]
        public void Handle_ExecutesAllPhases()
        {
            var repo = new InMemoryRepo();
            var effectSink = new TestEffectSink();
            var factory = new TestActorFactory();
            var gameFactory = new StubGameFactory();

            // Descriptors
            var t1 = TileId.New();
            var t2 = TileId.New();

            var spatial = new TurnForge.Engine.Commands.LoadGame.Descriptors.DiscreteSpatialDescriptor(
                new List<TileId> { t1, t2 },
                new List<TurnForge.Engine.Commands.LoadGame.Descriptors.DiscreteConnectionDeacriptor>()
            );
            var zones = new List<ZoneDescriptor>();
            var props = new List<PropDescriptor>();
            var unit1 = new AgentDescriptor(new AgentTypeId("u1"), new Position(t2));

            // Decisions
            var agentDecision = new AgentSpawnDecision(unit1.TypeId, new Position(t2), new List<IActorBehaviour>());
            var agentStrategy = new StubAgentStrategy(new[] { agentDecision });
            var propStrategy = new StubPropStrategy(new List<PropSpawnDecision>());

            var handler = new InitGameCommandHandler(factory, gameFactory, repo, propStrategy, agentStrategy, effectSink);

            var command = new InitGameCommand(spatial, zones, props, new List<AgentDescriptor> { unit1 });
            var result = handler.Handle(command);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Tags, Contains.Item("GameInitialized"));
            Assert.That(result.Tags, Contains.Item("StartFSM"));

            // Verify Agents Spawned (Phase 3)
            Assert.That(repo.State.GetAgents().Count, Is.EqualTo(1));
            // Use GetComponent to check position as it's the new standard
            Assert.That(repo.State.GetAgents().First().GetComponent<TurnForge.Engine.Entities.Components.PositionComponent>().CurrentPosition,
                Is.EqualTo(new Position(t2)));
        }
    }
}
