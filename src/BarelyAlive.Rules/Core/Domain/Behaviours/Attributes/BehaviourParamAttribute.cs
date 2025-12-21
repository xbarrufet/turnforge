namespace BarelyAlive.Rules.Core.Behaviours.Attributes;



[AttributeUsage(AttributeTargets.Parameter)]
public sealed class BehaviourParamAttribute : Attribute
{
    public string JsonName { get; }
    public BehaviourParamAttribute(string jsonName) => JsonName = jsonName;
}