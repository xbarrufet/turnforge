using BarelyAlive.Rules.Core.Behaviours.Attributes;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Behaviours.ZoneBehaviours;

/// <summary>
/// Behaviour que marca una zona como interior.
/// </summary>
[ZoneBehaviour("Indoor")]
public sealed class IndoorZoneBehaviour : IZoneBehaviour
{
    public string Name => "Indoor";
}

