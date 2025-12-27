using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Definitions.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ZoneBehaviours;

/// <summary>
/// Behaviour que marca una zona como interior.
/// </summary>
[ZoneTrait("Indoor")]
public sealed class IndoorZoneTrait : ZoneTrait
{
    public string Name => "Indoor";
}

