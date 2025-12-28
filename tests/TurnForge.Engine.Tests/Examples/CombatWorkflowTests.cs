using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Core.Orchestrator;
using TurnForge.Engine.Definitions;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Decisions.Entity.Interfaces;
using TurnForge.Engine.Events;
using TurnForge.Engine.Definitions.Actors;

namespace TurnForge.Engine.Tests.Examples
{
    [TestFixture]
    public class CombatWorkflowTests
    {
        // =========================
        // 1. Context & Definitions
        // =========================
        
        private class CombatContext : WorkflowContext
        {
            public EntityId Attacker { get; init; }
            public EntityId Defender { get; init; }
            public int WeaponDamage { get; init; }

            public int HitSuccesses { get; set; }
            public int SaveSuccesses { get; set; }
            
            public int FinalDamage => Math.Max(0, (HitSuccesses - SaveSuccesses) * WeaponDamage);
        }

        private record DiceRollInput(int Successes) : IInputActionResult;

        // =========================
        // 2. Decisions (State Mutations)
        // =========================

        private record ApplyDamageDecision(EntityId Target, int Amount) : IDecision
        {
            public string OriginId => "Combat";
            public DecisionTiming Timing => DecisionTiming.Immediate;

            public GameState Apply(GameState state)
            {
                // In a real scenario, we would use Components. 
                // Since GameState in tests might rely on specific Entity types,
                // we'll demonstrate the "Mutation" by adding a "Damaged" flag 
                // (or checking if we can modify a mock actor).
                
                // For this test, we verify correctness by checking if the Decision was generated correctly.
                // But for full Phase 6 compliance, we should simulate state change.
                
                // Let's assume we update the actor in the dictionary with a clone.
                if (state.Agents.TryGetValue(Target, out var agent))
                {
                     // Simplified: Just returning state to satisfy contract, 
                     // as we verify the *Generation* of the decision in this workflow test.
                     // The Engine Test covers the Apply mechanism itself.
                     return state;
                }
                return state;
            }
        }
        
        // =========================
        // 3. Reactions (Business Logic)
        // =========================

        private class CalcDamageReaction : IReaction
        {
            public ReactionId Id => new("CalcDamage");

            public bool CanReact(WorkflowContext context) => true;

            public ReactionResult React(WorkflowContext context, IInputActionResult? input)
            {
                var ctx = (CombatContext)context;
                var damage = ctx.FinalDamage;

                if (damage > 0)
                {
                    // Create Decision
                    var decision = new ApplyDamageDecision(ctx.Defender, damage);
                    context.RecordDecision(decision);
                }

                return ReactionResult.NoChange(context);
            }
        }

        // =========================
        // 4. Nodes (Steps)
        // =========================

        private class RollNode : INode, IAcceptsInput<DiceRollInput>
        {
            public NodeId Id { get; }
            public INode? NextNode { get; set; }
            private readonly Action<CombatContext, int> _storeResult;

            public RollNode(string id, Action<CombatContext, int> storeResult)
            {
                Id = new NodeId(id);
                _storeResult = storeResult;
            }

            public ValidationResult Validate(WorkflowContext context) => ValidationResult.OkResult;

            public void MoveForward(WorkflowContext context, DiceRollInput input)
            {
                _storeResult((CombatContext)context, input.Successes);
            }
        }

        private class DamageNode : INode, IAcceptsReactions
        {
            public NodeId Id => new("ApplyDamage");
            public INode? NextNode { get; set; } // Can describe End
            public IReadOnlyCollection<IReaction> AllowedReactions { get; } = new[] { new CalcDamageReaction() };

            public ValidationResult Validate(WorkflowContext context) => ValidationResult.OkResult;
        }

        // =========================
        // 5. Workflow Definition
        // =========================

        private class CombatWorkflow : IWorkflow
        {
            public WorkflowId Id => new("Combat");
            public INode StartNode { get; }
            public IReadOnlyCollection<IReaction> GlobalReactions { get; } = new List<IReaction>();
            
            private readonly Dictionary<string, INode> _nodes = new();

            public CombatWorkflow()
            {
                // Steps: AttackRoll -> SaveRoll -> ApplyDamage -> End
                
                var applyDamage = new DamageNode(); // End (Next=null)
                
                var saveRoll = new RollNode("SaveRoll", (ctx, val) => ctx.SaveSuccesses = val)
                {
                    NextNode = applyDamage
                };

                var attackRoll = new RollNode("AttackRoll", (ctx, val) => ctx.HitSuccesses = val)
                {
                    NextNode = saveRoll
                };
                
                StartNode = attackRoll;
                
                _nodes[attackRoll.Id.Value] = attackRoll;
                _nodes[saveRoll.Id.Value] = saveRoll;
                _nodes[applyDamage.Id.Value] = applyDamage;
            }

            public INode GetNode(NodeId nodeId) => _nodes[nodeId.Value];
        }

        // =========================
        // TEST CASE
        // =========================

        [Test]
        public void Execute_CombatWorkflow_ShouldGenerateDamageDecision()
        {
            // Arrange
            var orchestrator = new WorkflowOrchestrator();
            var workflow = new CombatWorkflow();
            var context = new CombatContext
            {
                Attacker = new EntityId(Guid.NewGuid()),
                Defender = new EntityId(Guid.NewGuid()),
                WeaponDamage = 2
            };

            // Act 1: Start Workflow (Suspends at AttackRoll)
            var result1 = orchestrator.Execute(workflow, context);
            Assert.That(result1.Status, Is.EqualTo(WorkflowStatus.Suspended));
            Assert.That(context.CurrentNodeId?.Value, Is.EqualTo("AttackRoll"));

            // Act 2: Provide Attack Roll (3 successes) -> Suspends at SaveRoll
            var result2 = orchestrator.Resume(workflow, context, new DiceRollInput(3));
            Assert.That(result2.Status, Is.EqualTo(WorkflowStatus.Suspended));
            Assert.That(context.CurrentNodeId?.Value, Is.EqualTo("SaveRoll"));

            // Act 3: Provide Save Roll (1 success) -> Calculates (3-1)*2 = 4 Damage -> Completes
            var result3 = orchestrator.Resume(workflow, context, new DiceRollInput(1));
            
            // Assert
            Assert.That(result3.Status, Is.EqualTo(WorkflowStatus.Completed));
            
            // Verify Logic
            Assert.That(context.HitSuccesses, Is.EqualTo(3));
            Assert.That(context.SaveSuccesses, Is.EqualTo(1));
            Assert.That(context.FinalDamage, Is.EqualTo(4));
            
            // Verify Decision Generation (Atomic State Application)
            Assert.That(context.Decisions.Count, Is.EqualTo(1));
            var decision = context.Decisions[0] as ApplyDamageDecision;
            Assert.That(decision, Is.Not.Null);
            Assert.That(decision.Target, Is.EqualTo(context.Defender));
            Assert.That(decision.Amount, Is.EqualTo(4));
        }
    }
}
