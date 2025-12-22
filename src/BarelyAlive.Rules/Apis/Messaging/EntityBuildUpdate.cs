using System.Collections.Generic;
using BarelyAlive.Rules.Core.Domain.ValueObjects;

namespace BarelyAlive.Rules.Apis.Messaging;

/// <summary>
/// Informació per crear una nova representació visual d'una entitat.
/// </summary>
public sealed record EntityBuildUpdate(
    string EntityId,
    string EntityName,
    string EntityCategory,    // Ex: "Hero", "Prop", "Enemy"
    string TileId,    // Posició inicial al món
    IReadOnlyList<string> Behaviors,
    IReadOnlyDictionary<string, object> InitialState // Stats o flags d'inici
);