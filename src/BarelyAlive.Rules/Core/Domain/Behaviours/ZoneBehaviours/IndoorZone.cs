using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ZoneBehaviours;

/// <summary>
/// Behaviour que marca una zona como interior.
/// </summary>
[ZoneTrait("Indoor")]
public sealed class IndoorZoneTrait : ZoneTrait
{
    public string Name => "Indoor";
}

