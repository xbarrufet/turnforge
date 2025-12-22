using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Descriptors;


public sealed record ActorBehaviourDescriptor(string Name, IReadOnlyDictionary<string,string> Attributes) 
{
}
