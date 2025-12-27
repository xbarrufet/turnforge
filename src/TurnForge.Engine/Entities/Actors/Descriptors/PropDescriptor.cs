using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Definitions.Descriptors.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Definitions.Actors.Descriptors;

public class PropDescriptor(  string definitionId):GameEntityBuildDescriptor(definitionId);