namespace BarelyAlive.Rules.Apis.Messaging;

/// <summary>
/// Snapshot of current game state with domain-specific collections.
/// Survivors and Zombies are separated for easier UI consumption.
/// </summary>
public record GameStateSnapshot
{
    public string CurrentStateId { get; init; } = string.Empty;
    public List<SurvivorDto> Survivors { get; init; } = new();
    public List<ZombieDto> Zombies { get; init; } = new();
    public List<PropDto> Props { get; init; } = new();
    public BoardDto? Board { get; init; }
}

/// <summary>
/// DTO representing a Survivor agent.
/// </summary>
public record SurvivorDto
{
    public string Id { get; init; } = string.Empty;
    public string TypeId { get; init; } = string.Empty;
    public TileReference Position { get; init; } = default!;
}

/// <summary>
/// DTO representing a Zombie agent.
/// </summary>
public record ZombieDto
{
    public string Id { get; init; } = string.Empty;
    public string TypeId { get; init; } = string.Empty;
    public TileReference Position { get; init; } = default!;
}

/// <summary>
/// DTO representing a Prop entity.
/// </summary>
public record PropDto
{
    public string Id { get; init; } = string.Empty;
    public string TypeId { get; init; } = string.Empty;
    public TileReference Position { get; init; } = default!;
}

/// <summary>
/// DTO representing board metadata.
/// </summary>
public record BoardDto
{
    public int TileCount { get; init; }
    public int ZoneCount { get; init; }
}
