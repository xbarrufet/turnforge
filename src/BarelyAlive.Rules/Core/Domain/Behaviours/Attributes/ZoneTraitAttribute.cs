namespace BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ZoneTraitAttribute : Attribute
{
    public string Type { get; }
    public ZoneTraitAttribute(string type) => Type = type;
}
