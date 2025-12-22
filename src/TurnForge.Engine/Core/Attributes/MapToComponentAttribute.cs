namespace TurnForge.Engine.Core.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class MapToComponentAttribute(Type componentType, string? propertyName = null) : Attribute
{
    // El tipus de component on anirà la dada (ex: HealthComponent)
    public Type ComponentType { get; } = componentType;

    // El nom de la propietat dins del component (ex: "BaseHealth")
    // Si és null, el Mapper usarà el nom de la propietat original.
    public string? PropertyName { get; } = propertyName;
}