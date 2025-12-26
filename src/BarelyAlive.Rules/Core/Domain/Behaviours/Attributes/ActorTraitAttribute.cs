namespace BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ActorTraitAttribute : Attribute
{
    public string Type { get; }
    public ActorTraitAttribute(string type) => Type = type;
}