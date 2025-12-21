namespace BarelyAlive.Rules.Apis.Messaging;

using System.Collections.Generic;

/// <summary>
/// Esdeveniments de domini que disparen reaccions visuals o sonores.
/// </summary>
public sealed record DomainEvent(
    string EventName,      // Ex: "OnCriticalHit", "OnLevelUp", "OnAbilityUsed"
    string SourceEntityId, // Qui dispara l'esdeveniment
    string? TargetEntityId,// Objectiu (opcional)
    IReadOnlyDictionary<string, object>? Metadata // Dades extra (ex: "Element": "Fire")
);