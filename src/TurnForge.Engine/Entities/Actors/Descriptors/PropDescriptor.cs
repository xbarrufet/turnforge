using TurnForge.Engine.Definitions.Actors;
using TurnForge.Engine.Definitions.Actors.Interfaces;
using TurnForge.Engine.Definitions.Descriptors;
using TurnForge.Engine.Definitions.Descriptors.Interfaces;
using TurnForge.Engine.ValueObjects;

using TurnForge.Engine.Entities.Actors;

using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.Descriptors.Interfaces;


namespace TurnForge.Engine.Entities.Actors.Descriptors;

public class PropDescriptor(  string definitionId):GameEntityBuildDescriptor(definitionId);