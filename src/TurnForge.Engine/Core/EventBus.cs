namespace TurnForge.Engine.Core;

public sealed class EventBus
{
    public event Action<object>? OnEvent;

    public void Publish(object evt)
        => OnEvent?.Invoke(evt);
}