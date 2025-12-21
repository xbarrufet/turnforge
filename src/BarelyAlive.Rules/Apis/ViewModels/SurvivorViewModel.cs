namespace BarelyAlive.Rules.Apis.ViewModels;

public record SurvivorViewModel(
    string Id,
    string Name,
    int HealthPercent,
    string PortraitPath
);
