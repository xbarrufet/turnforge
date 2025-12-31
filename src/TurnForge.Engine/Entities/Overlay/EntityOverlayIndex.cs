namespace TurnForge.Engine.Entities.Overlay;

public sealed class EntityOverlayIndex
{
    public SpawnEntityOperation? Spawn { get; private set; }
    public MoveOperation? LatestMove { get; private set; }
    
    public void Add(IGameStateOperation op)
    {
        switch (op)
        {
            case SpawnEntityOperation s:
                Spawn = s;
                break;
            case MoveOperation m:
                LatestMove = m;
                break;
        }
    }
}