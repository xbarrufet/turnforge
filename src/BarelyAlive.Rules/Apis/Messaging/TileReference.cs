namespace BarelyAlive.Rules.Apis.Messaging;

/// <summary>
/// Simple tile reference for DTOs. Just contains tile ID.
/// Different from TileDto which is for full tile information.
/// </summary>
public record TileReference(string Id);
