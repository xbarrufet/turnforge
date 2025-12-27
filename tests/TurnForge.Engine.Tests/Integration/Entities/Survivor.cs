using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Tests.Integration.Entities;

/// <summary>
/// Example of a specialized Agent type with custom components.
/// Survivors always have a Faction component.
/// </summary>
public class Survivor : Agent
{
    public Survivor(
        EntityId id, 
        string definitionId, 
        string name, 
        string category) 
        : base(id, name, category, definitionId)
    {
        // Survivors always have a Faction component
        AddComponent(new FactionComponent());
    }
}

/// <summary>
/// Custom component for faction/team affiliation
/// </summary>
public class FactionComponent : IFactionComponent
{
    public string Faction { get; set; } = string.Empty;
}

/// <summary>
/// Interface for faction component
/// </summary>
public interface IFactionComponent : IGameEntityComponent
{
    string Faction { get; set; }
}