namespace BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;



[AttributeUsage(AttributeTargets.Parameter)]
public sealed class BehaviourParamAttribute : Attribute
{
    public string JsonName { get; }
    public BehaviourParamAttribute(string jsonName) => JsonName = jsonName;
}