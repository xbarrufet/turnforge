using TurnForge.Engine.Commands.Spawn;
using TurnForge.Engine.Core.Workflow;
using TurnForge.Engine.Core.Workflow.Interfaces;
using TurnForge.Engine.Definitions.Actors.Descriptors;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;
using TurnForge.Engine.ValueObjects;
using TurnForge.Engine.Workflows.Spawn;
using TurnForge.Engine.Workflows.Spawn.Nodes;
using Moq;

namespace TurnForge.Engine.Tests.Workflows.Spawn;

[TestFixture]
public class SpawnWorkflowTests
{
    private WorkflowOrchestrator _orchestrator;
    private Mock<IGameCatalog> _catalogMock;

    [SetUp]
    public void SetUp()
    {
        _orchestrator = new WorkflowOrchestrator();
        _catalogMock = new Mock<IGameCatalog>();
    }

    [Test]
    public void SpawnWorkflow_ShouldCancel_WhenNoRequests()
    {
        // Arrange
        var workflow = new SpawnWorkflow();
        var context = new SpawnWorkflowContext(
            new List<SpawnRequest>(), 
            _catalogMock.Object);

        // Act
        var result = _orchestrator.Execute(workflow, context);

        // Assert
        Assert.That(result.Status, Is.EqualTo(WorkflowStatus.Cancelled));
    }

    [Test]
    public void SpawnWorkflow_ShouldComplete_WhenValidRequests()
    {
        // Arrange
        var definition = new TurnForge.Engine.Definitions.BaseGameEntityDefinition(
            "TestAgent", "Test");
        
        _catalogMock
            .Setup(c => c.GetDefinition<TurnForge.Engine.Definitions.BaseGameEntityDefinition>("TestAgent"))
            .Returns(definition);

        var request = new SpawnRequest("TestAgent");
        var workflow = new SpawnWorkflow();
        var context = new SpawnWorkflowContext(
            new List<SpawnRequest> { request }, 
            _catalogMock.Object);

        // Act
        var result = _orchestrator.Execute(workflow, context);

        // Assert
        Assert.That(result.Status, Is.EqualTo(WorkflowStatus.Completed));
        Assert.That(context.Decisions, Has.Count.GreaterThan(0));
    }

    [Test]
    public void SpawnWorkflow_ShouldProcessMultipleRequests()
    {
        // Arrange
        var definition = new TurnForge.Engine.Definitions.BaseGameEntityDefinition(
            "TestAgent", "Test");
        
        _catalogMock
            .Setup(c => c.GetDefinition<TurnForge.Engine.Definitions.BaseGameEntityDefinition>(It.IsAny<string>()))
            .Returns(definition);

        var requests = new List<SpawnRequest>
        {
            new SpawnRequest("Agent1") { Count = 2 },
            new SpawnRequest("Agent2") { Count = 3 }
        };
        
        var workflow = new SpawnWorkflow();
        var context = new SpawnWorkflowContext(requests, _catalogMock.Object);

        // Act
        var result = _orchestrator.Execute(workflow, context);

        // Assert
        Assert.That(result.Status, Is.EqualTo(WorkflowStatus.Completed));
        Assert.That(context.Decisions.Count, Is.EqualTo(5));
    }

    [Test]
    public void SpawnWorkflow_Nodes_ShouldBeChainedCorrectly()
    {
        // Arrange
        var workflow = new SpawnWorkflow();

        // Assert node chain
        Assert.That(workflow.StartNode, Is.TypeOf<SpawnValidationNode>());
        Assert.That(workflow.StartNode.NextNode, Is.TypeOf<SpawnProcessingNode>());
        Assert.That(workflow.StartNode.NextNode!.NextNode, Is.TypeOf<SpawnPlacementNode>());
        Assert.That(workflow.StartNode.NextNode!.NextNode!.NextNode, Is.TypeOf<SpawnDecisionNode>());
        Assert.That(workflow.StartNode.NextNode!.NextNode!.NextNode!.NextNode, Is.Null);
    }

    [Test]
    public void SpawnWorkflow_GetNode_ShouldReturnCorrectNodes()
    {
        // Arrange
        var workflow = new SpawnWorkflow();

        // Act & Assert
        Assert.That(workflow.GetNode(new NodeId("Spawn.Validation")), Is.TypeOf<SpawnValidationNode>());
        Assert.That(workflow.GetNode(new NodeId("Spawn.Processing")), Is.TypeOf<SpawnProcessingNode>());
        Assert.That(workflow.GetNode(new NodeId("Spawn.Placement")), Is.TypeOf<SpawnPlacementNode>());
        Assert.That(workflow.GetNode(new NodeId("Spawn.Decision")), Is.TypeOf<SpawnDecisionNode>());
    }
}
