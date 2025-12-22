using System.Collections.Generic;
using BarelyAlive.Rules.Apis.Messaging;
using Godot;

namespace BarelyAlive.Godot.Model;

public partial class ViewModel:Node
{
    [Signal]
    public delegate void AvailableSurvivorsChangedEventHandler();

    private List<SurvivorDefinition> _survivorsAvailableInCatalog = new();
    public IReadOnlyList<SurvivorDefinition> AvailableSurvivors => _survivorsAvailableInCatalog;

    public List<Survivor> Survivor = []; 
    
    public void SetAvailableSurvivorsInCatalog(List<SurvivorDefinition> survivors)
    {
        _survivorsAvailableInCatalog = survivors;
        EmitSignal(SignalName.AvailableSurvivorsChanged);
    }
    
    public void SetStartingSurvivors(List<SurvivorDefinition> survivors)
    {
        // Implementation for setting starting survivors, it's the start of the game
        // stores the Survivors in the ViewModel as View Source of Truth
    }
}