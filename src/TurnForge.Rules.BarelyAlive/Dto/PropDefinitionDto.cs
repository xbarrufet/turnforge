namespace TurnForge.Rules.BarelyAlive.Dto;

public class PropDefinitionDto
{
    public string TypeId { get; set; } = string.Empty;
    public int? MaxHealth { get; set; }
    public List<TraitDto> Traits { get; set; } = new();
}
