using TurnForge.Engine.Entities.Board.Interfaces;

namespace BarelyAlive.Rules.Core.Behaviours.ZoneBehaviours;

/// <summary>
/// Behaviour que marca una zona como oscura.
/// </summary>
public sealed class DarkZoneBehaviour : IZoneBehaviour
{
    public string Name => "Dark";
}

