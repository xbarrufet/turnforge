using System;

namespace TurnForge.Engine.Core.Interfaces;

public interface IGameLogger
{
    void Log(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? exception = null);
}
