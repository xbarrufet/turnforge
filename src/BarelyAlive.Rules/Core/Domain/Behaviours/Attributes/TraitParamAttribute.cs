namespace BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;



[AttributeUsage(AttributeTargets.Parameter)]
public sealed class TraitParamAttribute : Attribute
{
    public string JsonName { get; }
    public TraitParamAttribute(string jsonName) => JsonName = jsonName;
}