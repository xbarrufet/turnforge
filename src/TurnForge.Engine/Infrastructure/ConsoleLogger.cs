using System;
using TurnForge.Engine.Core.Interfaces;

namespace TurnForge.Engine.Infrastructure;

public sealed class ConsoleLogger : IGameLogger
{
    public void Log(string message) => Console.WriteLine($"[INFO] {message}");
    public void LogWarning(string message) => Console.WriteLine($"[WARN] {message}");
    public void LogError(string message, Exception? exception = null) 
    {
        Console.WriteLine($"[ERROR] {message}");
        if (exception != null) Console.WriteLine(exception);
    }
}
