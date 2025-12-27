using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Definitions.Board;
using TurnForge.Engine.Definitions.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ZoneBehaviours;

/// <summary>
/// Behaviour que marca una zona como oscura.
/// </summary>
[ZoneTrait("Dark")]
public sealed class DarkZoneTrait : ZoneTrait
{
    public string Name => "Dark";
}

