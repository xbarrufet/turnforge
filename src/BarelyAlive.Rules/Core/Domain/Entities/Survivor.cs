using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.Core.Attributes;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.ValueObjects;
using BarelyAlive.Rules.Core.Domain.Descriptors;

namespace BarelyAlive.Rules.Core.Domain.Entities;

/// <summary>
/// Survivor entity - playable characters in BarelyAlive.
/// </summary>
/// <remarks>
/// This class declares its type relationships via attributes:
/// - [DefinitionType] links to SurvivorDefinition
/// - [DescriptorType] links to SurvivorDescriptor for type-safe spawn processing
/// 
/// These attributes enable:
/// 1. Compile-time safety (missing types cause build errors)
/// 2. Runtime type lookup via EntityTypeRegistry
/// 3. Type-specific spawn strategy processing
/// </remarks>
[DefinitionType(typeof(SurvivorDefinition))]
[DescriptorType(typeof(SurvivorDescriptor))]
public class Survivor : Agent
{
    public Survivor(EntityId id, string definitionId, string name, string category) 
        : base(id, definitionId, name, category)
    {
    }
}

/// <summary>
/// Definition for Survivor entities.
/// Contains default property values loaded from data files.
/// </summary>
public class SurvivorDefinition : BaseGameEntityDefinition
{
    public int MaxHealth { get; set; } = 12;
    public string Faction { get; set; } = "Player";
    public int ActionPoints { get; set; } = 3;
    
    public SurvivorDefinition(string definitionId) 
        : base(definitionId)
    {
        // Using int constructor if available or parameterless
        // Default traits can be added here if needed, but Identity is usually external
        Traits.Add(new TurnForge.Engine.Traits.Standard.VitalityTrait(maxHP: MaxHealth));
    }
}