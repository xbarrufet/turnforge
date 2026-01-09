using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Definitions;

public interface IEntityDefinition
{
    public string DefinitionId { get; set; }
    public string Name { get; set; }
    public Category Category { get; set; }
    
    public void AddRequiredTrait<TTrait>(TTrait trait) where TTrait : class;
    public void AddRequiredComponent<TComponent>(TComponent component) where TComponent : class;
    
    public IEnumerable<TComponent> GetRequiredComponents<TComponent>() where TComponent : class;
    public IEnumerable<TTrait> GetRequiredTraits<TTrait>() where TTrait : class;
    
    public bool TryGetTrait<TTrait>(out TTrait trait) where TTrait : class;
    public bool TryGetComponent<TComponent>(out TComponent component) where TComponent : class;
    
    public bool HasTrait<TTrait>() where TTrait : class;
    public bool HasComponent<TComponent>() where TComponent : class;
    
    public void AddTrait<TTrait>(TTrait trait) where TTrait : class;
    public void AddComponent<TComponent>(TComponent component) where TComponent : class;
    
}