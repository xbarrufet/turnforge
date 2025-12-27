using TurnForge.Engine.ValueObjects;


namespace TurnForge.Engine.Traits.Standard;

/// <summary>
/// Represents a damage profile for weapons or abilities.
/// A single weapon can have multiple profiles (e.g., shotgun + grenade launcher).
/// </summary>
public record DamageProfile(
    string Name,                      // Profile name (e.g., "Shotgun", "GrenadeLauncher")
    PotentialRandomValue Damage,      // Damage value or dice
    string Category = "Physical"      // Damage type (Physical, Fire, Explosive, etc.)
);

/// <summary>
/// Trait that defines damage capabilities with support for multiple profiles.
/// </summary>
/// <remarks>
/// This is a pure data trait (BaseTrait) rather than a component trait
/// because multiple profiles don't map 1:1 to a single component.
/// 
/// Example usage:
/// <code>
/// var weaponDamage = new DamageTrait()
///     .AddProfile("Primary", "2d6", "Physical")
///     .AddProfile("GrenadeLauncher", "3d8", "Explosive");
/// </code>
/// </remarks>
public class DamageTrait : BaseTrait
{
    private readonly List<DamageProfile> _profiles = new();
    
    /// <summary>
    /// All damage profiles for this trait.
    /// </summary>
    public IReadOnlyList<DamageProfile> Profiles => _profiles.AsReadOnly();
    
    /// <summary>
    /// The primary (first) damage profile, or null if none.
    /// </summary>
    public DamageProfile? Primary => _profiles.Count > 0 ? _profiles[0] : null;
    
    /// <summary>
    /// Shortcut to primary damage value.
    /// </summary>
    public PotentialRandomValue Damage => Primary?.Damage ?? PotentialRandomValue.Fixed(0);
    
    /// <summary>
    /// Shortcut to primary damage category.
    /// </summary>
    public string DamageCategory => Primary?.Category ?? "Physical";
    
    // ─────────────────────────────────────────────────────────────
    // Constructors
    // ─────────────────────────────────────────────────────────────
    
    public DamageTrait() { }
    
    /// <summary>
    /// Creates a DamageTrait with a single profile.
    /// </summary>
    public DamageTrait(PotentialRandomValue damage, string category = "Physical")
    {
        AddProfile("Primary", damage, category);
    }
    
    /// <summary>
    /// Creates a DamageTrait with a named single profile.
    /// </summary>
    public DamageTrait(string name, PotentialRandomValue damage, string category = "Physical")
    {
        AddProfile(name, damage, category);
    }
    
    // ─────────────────────────────────────────────────────────────
    // Fluent Profile Management
    // ─────────────────────────────────────────────────────────────
    
    /// <summary>
    /// Adds a damage profile.
    /// </summary>
    public DamageTrait AddProfile(string name, PotentialRandomValue damage, string category = "Physical")
    {
        _profiles.Add(new DamageProfile(name, damage, category));
        return this;
    }
    
    /// <summary>
    /// Adds a damage profile.
    /// </summary>
    public DamageTrait AddProfile(DamageProfile profile)
    {
        _profiles.Add(profile);
        return this;
    }
    
    /// <summary>
    /// Gets a profile by name.
    /// </summary>
    public DamageProfile? GetProfile(string name) 
        => _profiles.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    
    /// <summary>
    /// Checks if a profile with the given name exists.
    /// </summary>
    public bool HasProfile(string name) 
        => _profiles.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}
