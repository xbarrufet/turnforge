using BarelyAlive.Rules.Apis.Messaging;
using Godot;

namespace BarelyAlive.Godot.Model;

public class Survivor
{
    public string Id;
    public string Name;
    public Texture Avatar;
    public int CurrentHealth;
    public int RemainingActionPoints;
    public string TileId;


    public Survivor(SurvivorDefinition survivorDefinition)
    {
        Id = survivorDefinition.Id;
        Name = survivorDefinition.Name; 
        CurrentHealth = survivorDefinition.MaxHealth;
        RemainingActionPoints = 3; //TODO

    }
    
    
}