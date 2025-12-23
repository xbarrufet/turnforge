namespace BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ActorBehaviourAttribute : Attribute
{
    public string Type { get; }
    public ActorBehaviourAttribute(string type) => Type = type;
}