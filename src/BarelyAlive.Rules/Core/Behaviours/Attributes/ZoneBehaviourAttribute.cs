namespace BarelyAlive.Rules.Core.Behaviours.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ZoneBehaviourAttribute : Attribute
{
    public string Type { get; }
    public ZoneBehaviourAttribute(string type) => Type = type;
}
