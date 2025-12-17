using TurnForge.Engine.Entities.Actors.Interfaces;

namespace TurnForge.Engine.Entities.Actors.Definitions;


public sealed record ActorTraitDescriptor(string Name, IReadOnlyDictionary<string,string> Attributes) 
{
}
