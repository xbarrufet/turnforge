namespace BarelyAlive.Rules.Apis.Messaging;

/// <summary>
/// Representa un canvi en una dada específica d'una entitat existent.
/// </summary>
public sealed record EntityStateUpdate(
    string EntityId,
    string ComponentType, // El nom de la propietat o component (ex: "Health", "Mana")
    object NewValue,      // El nou valor processat
    object? Delta         // La diferència (ex: -10), útil per a popups visuals
);