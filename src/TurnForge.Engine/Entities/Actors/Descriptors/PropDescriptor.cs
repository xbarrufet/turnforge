using TurnForge.Engine.Entities.Actors;
using TurnForge.Engine.Entities.Actors.Interfaces;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Entities.Actors.Descriptors;

public class PropDescriptor(  string definitionId):GameEntityBuildDescriptor(definitionId);