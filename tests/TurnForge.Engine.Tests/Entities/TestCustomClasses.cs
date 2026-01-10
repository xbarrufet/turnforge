using TurnForge.Engine.Entities.Definitions.Actors;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Players.ValueObjects;
using TurnForge.Engine.Entities.TraitsComponents.Components;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;
using TurnForge.Engine.Entities.TraitsComponents.Traits;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Entities;


public sealed class CustomSpeedTrait : ITrait
{
    private int _maxSpeed;
    public int MaxSpeed
    {
        get => _maxSpeed;
        init
        {
            _maxSpeed = value;
            IsInitialized = true;
        }
    }

    public bool IsInitialized { get; private set; }
    public bool StackAllowed { get; init; } = false;

    public CustomSpeedTrait()
    {
    }

    public CustomSpeedTrait(int maxSpeed)
    {
        MaxSpeed = maxSpeed;
        // IsInitialized gets set by MaxSpeed init setter
    }
}
public sealed class CustomSpeedComponent : IGameEntityComponent
{
    public int MaxSpeed { get; }
    public int CurrentSpeed { get; set; }

    // TraitInitializationService will discover this constructor and materialize the component from the trait
    public CustomSpeedComponent(CustomSpeedTrait trait)
    {
        MaxSpeed = trait.MaxSpeed;
        CurrentSpeed = 0;
    }

    public CustomSpeedComponent(CustomSpeedTrait trait, int currentSpeed)
    {
        MaxSpeed = trait.MaxSpeed;
        CurrentSpeed = currentSpeed;
    }

    public bool IsInitialized => true;
}

public class CustomSpeedInitializedAutomaticallyDefinition : AgentDefinition
{
    public CustomSpeedInitializedAutomaticallyDefinition(string definitionId) : base(definitionId)
    {
        AddTrait(new VitalityTrait(10));
        // HealthComponent will be created automatically by ComponentInitializationService
        AddTrait(new CustomSpeedTrait { MaxSpeed = 9 }, true);
    }
}

public class CustomSpeedAgentRequiresInitializationDefinition : AgentDefinition
{
    public CustomSpeedAgentRequiresInitializationDefinition(string definitionId) : base(definitionId)
    {
        AddTrait(new VitalityTrait(10));
        AddTrait(new CustomSpeedTrait(), true);
    }
}

public class CustomTestAgentDescriptor : AgentDescriptor
{
    public CustomTestAgentDescriptor(
        string definitionId,
        string teamId,
        PlayerId playerId,
        IBoardPositionId startPosition)
        : base(definitionId, teamId, playerId, startPosition, definitionId)
    {
        // Optional: override traits or add extra components here
        // DefinitionTraitValues.Add(new CustomSpeedTrait { MaxSpeed = 12 });
        // ExtraComponents.Add(new ActionPoolComponent());
    }

    public CustomTestAgentDescriptor(
        string definitionId,
        string teamId,
        PlayerId playerId,
        IBoardPositionId startPosition,
        ITrait trait)
        : base(teamId, playerId, startPosition, definitionId: definitionId, new List<IGameEntityComponent> { },
            new List<ITrait> { trait })
    {
        // Optional: override traits or add extra components here
        // DefinitionTraitValues.Add(new CustomSpeedTrait { MaxSpeed = 12 });
        // ExtraComponents.Add(new ActionPoolComponent());
    }

    public CustomTestAgentDescriptor(
        string definitionId,
        string teamId,
        PlayerId playerId,
        IBoardPositionId startPosition,
        CustomSpeedComponent customSpeedComponent,
        ITrait trait)
        : base(teamId, playerId, startPosition, definitionId: definitionId, new List<IGameEntityComponent> { customSpeedComponent },
            new List<ITrait> { trait })
    {
        // Optional: override traits or add extra components here
        // DefinitionTraitValues.Add(new CustomSpeedTrait { MaxSpeed = 12 });
        // ExtraComponents.Add(new ActionPoolComponent());
    }
}


