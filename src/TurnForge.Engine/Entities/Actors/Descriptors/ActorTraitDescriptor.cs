using TurnForge.Engine.Definitions.Actors.Interfaces;

namespace TurnForge.Engine.Definitions.Actors.Descriptors;


public sealed record ActorTraitDescriptor(string Name, IReadOnlyDictionary<string,string> Attributes) 
{
}
