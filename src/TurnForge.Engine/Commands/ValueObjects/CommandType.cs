namespace TurnForge.Engine.Commands.ValueObjects;

public readonly record struct CommandType(string Name)
{
    public static readonly CommandType StartGame = new("StartGame");
    public static readonly CommandType ACK = new("ACK");
    
}   