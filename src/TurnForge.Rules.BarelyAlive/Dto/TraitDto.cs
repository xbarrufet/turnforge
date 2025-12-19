namespace TurnForge.Rules.BarelyAlive.Dto;

public class TraitAttributeDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class TraitDto
{
    public string Type { get; set; } = string.Empty;
    public List<TraitAttributeDto> Attributes { get; set; } = new();
}
