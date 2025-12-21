using BarelyAlive.Rules.Core.Behaviours.Attributes;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Behaviours.ZoneBehaviours;

/// <summary>
/// Behaviour que marca una zona como oscura.
/// </summary>
[ZoneBehaviour("Dark")]
public sealed class DarkZoneBehaviour : ZoneBehaviour
{
    public string Name => "Dark";
}

