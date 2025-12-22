using TurnForge.Engine.Core.Interfaces;
using Godot;

namespace BarelyAlive.Godot.Adapter.Infrastructure;

public sealed class GodotLogger : IGameLogger
{
    public void Log(string message) => GD.Print($"[Engine] {message}");
    public void LogWarning(string message) => GD.PushWarning($"[Engine] {message}");
    public void LogError(string message, System.Exception? exception = null) 
    {
        GD.PrintErr($"[Engine] {message}");
        if (exception != null)
        {
             GD.PrintErr($"[Engine] Exception: {exception}");
        }
    }
}
