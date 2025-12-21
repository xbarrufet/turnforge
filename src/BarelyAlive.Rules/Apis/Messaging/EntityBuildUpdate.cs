using System.Collections.Generic;
using BarelyAlive.Rules.Core.Domain.ValueObjects;

namespace BarelyAlive.Rules.Apis.Messaging;

/// <summary>
/// Informació per crear una nova representació visual d'una entitat.
/// </summary>
public sealed record EntityBuildUpdate(
    string EntityId,
    string EntityType,    // Ex: "Hero", "Prop", "Enemy"
    string DescriptorId, // La clau de configuració (ex: "warrior_v1")
    Vector Position,    // Posició inicial al món
    IReadOnlyDictionary<string, object> InitialState // Stats o flags d'inici
);