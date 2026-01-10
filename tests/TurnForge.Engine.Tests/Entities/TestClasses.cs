using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Definitions.Board;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Entities;


public class TestIncompleteDescriptor:AgentDescriptor
{
    public TestIncompleteDescriptor(
        string teamId,
        PlayerId playerId) : 
        base(
            teamId:teamId,
            playerId:playerId)
    
    {
    }
}


public class TestAgentDescriptor : AgentDescriptor
{
    public TestAgentDescriptor(
        string teamId,
        PlayerId playerId,
        IBoardPositionId startPosition,
        string definitionId) : 
            base(
                teamId:teamId,
                playerId:playerId,
                startPosition:startPosition,
                definitionId:definitionId)
    {
        AddDefinitionTraitValue(new VitalityTrait(10));
    }
    
    public TestAgentDescriptor(
        string teamId,
        PlayerId playerId,
        IBoardPositionId startPosition) : 
        base(
            teamId:teamId,
            playerId:playerId,
            startPosition:startPosition)
    
    {
        AddDefinitionTraitValue(new VitalityTrait(10));
    }
    
    public TestAgentDescriptor(
        string teamId,
        PlayerId playerId) : 
        base(
            teamId:teamId,
            playerId:playerId)
    
    {
        AddDefinitionTraitValue(new VitalityTrait(10));
    }

    public TestAgentDescriptor(
        string name,
        string teamId,
        PlayerId playerId,
        IBoardPositionId startPosition,
        string definitionId) :
        base(
            name: name,
            teamId: teamId,
            playerId: playerId,
            startPosition: startPosition,
            definitionId: definitionId)
    {
    }
}

public class TestPropDescriptor : PropDescriptor
{
    public TestPropDescriptor(
        IBoardPositionId startPosition,
        string definitionId) : 
        base(
            startPosition:startPosition,
            definitionId:definitionId)
    {
        AddDefinitionTraitValue(new VitalityTrait(10));
    }
    
    public TestPropDescriptor(
        IBoardPositionId startPosition) : 
        base(
            startPosition:startPosition)
    
    {
        AddDefinitionTraitValue(new VitalityTrait(10));
    }
    
    public TestPropDescriptor() : 
        base()
    
    {
        AddDefinitionTraitValue(new VitalityTrait(10));
    }

    public TestPropDescriptor(
        string name,
        IBoardPositionId startPosition,
        string definitionId) :
        base(
            name: name,
            startPosition: startPosition,
            definitionId: definitionId)
    {
        AddDefinitionTraitValue(new VitalityTrait(10));
        
    }
     





}