using System;
using TurnForge.Engine.Core.Logging;

namespace TurnForge.Engine.Core.Interfaces;

public interface IGameLogger
{
    /// <summary>
    /// Structured log with level and optional context.
    /// </summary>
    void Log(LogLevel level, string message, LogContext? context = null);

    // Convenience methods
    void LogDebug(string message, LogContext? context = null) => Log(LogLevel.Debug, message, context);
    void LogInfo(string message, LogContext? context = null) => Log(LogLevel.Info, message, context);
    void LogWarning(string message, LogContext? context = null) => Log(LogLevel.Warning, message, context);
    void LogError(string message, Exception? exception = null, LogContext? context = null);

    // Legacy support (deprecated, will be removed)
    [Obsolete("Use Log(LogLevel, string, LogContext?) instead")]
    void Log(string message) => Log(LogLevel.Info, message);
}
