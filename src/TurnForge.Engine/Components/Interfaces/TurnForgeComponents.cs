using TurnForge.Engine.Components.Interfaces;

public interface TurnForgeComponents {

    // Types cannot be const, so usually we cannot put them here for Attribute usage.
    // Attributes require typeof(Interface) directly.
    // However, we can keep them as static readonly for other usages, but NOT for attributes.
    // For now, I'll comment them out or remove them to avoid confusion, 
    // OR keep them as static getters but knowing they don't work in Attributes.
    

    // Strings CAN be const
    public const string Prop_HealthComponent_MaxHealth = "MaxHealth";
    public const string Prop_PositionComponent_CurrentPosition = "CurrentPosition";

}