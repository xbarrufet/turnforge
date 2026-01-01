using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities;

public interface IGameEntity
{
    public EntityId Id { get; }
    public string Name { get; }
    public string Category { get; }
    public void AddComponent<T>(T component) where T : IGameEntityComponent;
    public bool HasComponent<T>() where T : class, IGameEntityComponent;
    public T GetRequiredComponent<T>() where T : class, IGameEntityComponent;
    public bool HasRequiredComponents();
}
