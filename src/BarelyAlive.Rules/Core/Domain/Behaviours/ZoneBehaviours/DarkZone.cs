using BarelyAlive.Rules.Core.Domain.Behaviours.Attributes;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Domain.Behaviours.ZoneBehaviours;

/// <summary>
/// Behaviour que marca una zona como oscura.
/// </summary>
[ZoneTrait("Dark")]
public sealed class DarkZoneTrait : ZoneTrait
{
    public string Name => "Dark";
}

