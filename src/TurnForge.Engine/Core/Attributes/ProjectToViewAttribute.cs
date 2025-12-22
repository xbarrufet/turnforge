namespace TurnForge.Engine.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ProjectToViewAttribute(string? viewKey = null) : Attribute
{
    // La clau que rebrà la UI (ex: "current_hp")
    // Si és null, s'usarà el nom de la propietat C# (ex: "CurrentHealth").
    public string? ViewKey { get; } = viewKey;
}