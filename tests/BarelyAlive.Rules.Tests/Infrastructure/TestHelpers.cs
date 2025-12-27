using BarelyAlive.Rules.Core.Domain.Entities;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Infrastructure.Catalog.Interfaces;

namespace BarelyAlive.Rules.Tests.Infrastructure;

public static class TestHelpers
{
    public const string MikeId = "Survivor.Mike";
    public const string DougId = "Survivor.Doug";
    public const string ZRunnerId = "Zombie.Runner";
    public const string ZFatId = "Zombie.Fat";
    public const string ZNormalId = "Zombie.Normal";
    public const string SpawnZombieId = "Spawn.Zombie";
    public const string SpawnPlayerId = "Spawn.Player";
    public const string DoorId = "Door";
    public const string AreaId = "Area";

    public static readonly SurvivorDefinition MikeDef = new(MikeId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Mike", "Survivor") } };
    public static readonly SurvivorDefinition DougDef = new(DougId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Doug", "Survivor") } };
    
    // Zombies
    public static readonly BaseGameEntityDefinition ZRunnerDef = new(ZRunnerId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Zombie Runner", "Zombie") } };
    public static readonly BaseGameEntityDefinition ZFatDef = new(ZFatId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Zombie Fat", "Zombie") } };
    public static readonly BaseGameEntityDefinition ZNormalDef = new(ZNormalId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Zombie Normal", "Zombie") } };
    
    // Spawn
    public static readonly BaseGameEntityDefinition SpawnPlayerDef = new(SpawnPlayerId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Spawn Player", "Spawn.Player") } };
    public static readonly BaseGameEntityDefinition SpawnZombieDef = new(SpawnZombieId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Spawn Zombie", "Spawn.Zombie") } };

    // Zones
    public static readonly BaseGameEntityDefinition AreaDef = new(AreaId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Tile", "Board") } };
    
    // Doors
    public static readonly DoorDefinition DoorDef = new(DoorId) { Traits = { new TurnForge.Engine.Traits.Standard.IdentityTrait("Door", "Connections") } };

    public static void RegisterTestEntities(this TurnForge.Engine.APIs.Interfaces.IGameCatalogApi catalog)
    {
        catalog.RegisterDefinition(MikeDef);
        catalog.RegisterDefinition(DougDef);
        catalog.RegisterDefinition(ZRunnerDef);
        catalog.RegisterDefinition(ZFatDef);
        catalog.RegisterDefinition(ZNormalDef);
        catalog.RegisterDefinition(SpawnPlayerDef);
        catalog.RegisterDefinition(SpawnZombieDef);
        catalog.RegisterDefinition(AreaDef);
        catalog.RegisterDefinition(DoorDef);
    }

    public const string Mission01Json = """
    {
      "missionName": "mission01",
    
      "scale": {
        "width": 250,
        "height": 250
      },
    
      "spatial": {
        "type": "DiscreteGraph",
        "nodes": [
          { "id": "d7de841d-64a5-48b3-9662-0fe757a8950e", "x": 0, "y": 0 },
          { "id": "dffe31ca-330e-404a-87f3-21555b600ede", "x": 1, "y": 0 },
          { "id": "dd05ee1d-7a6e-4996-b0bc-a9acb50fe3de", "x": 2, "y": 0 },
    
          { "id": "52aef277-2960-4974-8a29-e9e981d11c1a", "x": 2, "y": 1 },
          { "id": "c3e99a3c-0a24-42af-961e-1d8be2352bea", "x": 2, "y": 2 },
    
          { "id": "1bd502ad-cfcd-4a07-94a7-93634651a1c3", "x": 0, "y": 2 },
          { "id": "66a0dadc-d774-4ce9-a3ec-0213e9528af6", "x": 1, "y": 2 },
    
          { "id": "6792b7cd-01d7-4bf7-95b4-d2876d5b4020", "x": 0, "y": 1 },
          { "id": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53", "x": 1, "y": 1 }
        ],
    
        "connections": [
          { "id": "55f54395-6e94-4d1a-9694-824050f4a867", "from": "dffe31ca-330e-404a-87f3-21555b600ede", "to": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53", "direction": "South" },
          { "id": "a9010375-3e28-4e8a-8612-42998638641a", "from": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53", "to": "dffe31ca-330e-404a-87f3-21555b600ede", "direction": "North" },
    
          { "id": "5039f9b5-6a56-42d6-84ff-3773199c00b1", "from": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53", "to": "6792b7cd-01d7-4bf7-95b4-d2876d5b4020", "direction": "West" },
          { "id": "b3456729-1a4e-4f3b-a678-5c4289056234", "from": "6792b7cd-01d7-4bf7-95b4-d2876d5b4020", "to": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53", "direction": "East" },
    
          { "id": "c6789012-2b6f-4c5d-b89a-6d5390168345", "from": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53", "to": "66a0dadc-d774-4ce9-a3ec-0213e9528af6", "direction": "South" },
          { "id": "d7890123-3c7a-4d6e-c90b-7e6401279456", "from": "66a0dadc-d774-4ce9-a3ec-0213e9528af6", "to": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53", "direction": "North" },
    
          { "id": "e8901234-4d8b-4e7f-d01c-8f7512380567", "from": "66a0dadc-d774-4ce9-a3ec-0213e9528af6", "to": "c3e99a3c-0a24-42af-961e-1d8be2352bea", "direction": "East" },
          { "id": "f9012345-5e9c-4f8a-e12d-908623491678", "from": "c3e99a3c-0a24-42af-961e-1d8be2352bea", "to": "66a0dadc-d774-4ce9-a3ec-0213e9528af6", "direction": "West" },
    
          { "id": "12345678-0f0a-4b2c-834e-019734502789", "from": "d7de841d-64a5-48b3-9662-0fe757a8950e", "to": "dffe31ca-330e-404a-87f3-21555b600ede", "direction": "East" },
          { "id": "23456789-1a1b-4c3d-945f-120845613890", "from": "dffe31ca-330e-404a-87f3-21555b600ede", "to": "d7de841d-64a5-48b3-9662-0fe757a8950e", "direction": "West" },
    
          { "id": "34567890-2b2c-4d4e-a560-231956724901", "from": "dffe31ca-330e-404a-87f3-21555b600ede", "to": "dd05ee1d-7a6e-4996-b0bc-a9acb50fe3de", "direction": "East" },
          { "id": "45678901-3c3d-4e5f-b671-342067835012", "from": "dd05ee1d-7a6e-4996-b0bc-a9acb50fe3de", "to": "dffe31ca-330e-404a-87f3-21555b600ede", "direction": "West" },
    
          { "id": "56789012-4d4e-4f60-c782-453178946123", "from": "dd05ee1d-7a6e-4996-b0bc-a9acb50fe3de", "to": "52aef277-2960-4974-8a29-e9e981d11c1a", "direction": "South" },
          { "id": "67890123-5e5f-4071-d893-564289057234", "from": "52aef277-2960-4974-8a29-e9e981d11c1a", "to": "dd05ee1d-7a6e-4996-b0bc-a9acb50fe3de", "direction": "North" },
    
          { "id": "78901234-6f60-4182-e904-675390168345", "from": "52aef277-2960-4974-8a29-e9e981d11c1a", "to": "c3e99a3c-0a24-42af-961e-1d8be2352bea", "direction": "South" },
          { "id": "89012345-7071-4293-f015-786401279456", "from": "c3e99a3c-0a24-42af-961e-1d8be2352bea", "to": "52aef277-2960-4974-8a29-e9e981d11c1a", "direction": "North" }
        ],

        "zones": [
          {
            "id": "6afac418-e205-4125-839a-48452ec273e2",
            "bound": { "type": "TileSet", "tiles": ["d7de841d-64a5-48b3-9662-0fe757a8950e"] }
          },
          {
            "id": "7bfac418-e205-4125-839a-48452ec273e3",
            "bound": { "type": "TileSet", "tiles": ["dffe31ca-330e-404a-87f3-21555b600ede"] }
          },
          {
            "id": "8cfac418-e205-4125-839a-48452ec273e4",
            "bound": { "type": "TileSet", "tiles": ["dd05ee1d-7a6e-4996-b0bc-a9acb50fe3de"] }
          },
          {
            "id": "9dfac418-e205-4125-839a-48452ec273e5",
            "bound": { "type": "TileSet", "tiles": ["52aef277-2960-4974-8a29-e9e981d11c1a"] }
          },
          {
            "id": "aefac418-e205-4125-839a-48452ec273e6",
            "bound": { "type": "TileSet", "tiles": ["c3e99a3c-0a24-42af-961e-1d8be2352bea"] }
          },
          {
            "id": "befac418-e205-4125-839a-48452ec273e7",
            "bound": { "type": "TileSet", "tiles": ["1bd502ad-cfcd-4a07-94a7-93634651a1c3"] }
          },
          {
            "id": "66a0dadc-d774-4ce9-a3ec-0213e9528af6",
            "bound": { "type": "TileSet", "tiles": ["66a0dadc-d774-4ce9-a3ec-0213e9528af6"] }
          },
          {
            "id": "6792b7cd-01d7-4bf7-95b4-d2876d5b4020",
            "bound": { "type": "TileSet", "tiles": ["6792b7cd-01d7-4bf7-95b4-d2876d5b4020"] }
          },
          {
            "id": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53",
            "bound": { "type": "TileSet", "tiles": ["07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53"] }
          }
        ]
      },
      
      "props": [
        { "typeId": "Spawn.Zombie", "position": "d7de841d-64a5-48b3-9662-0fe757a8950e", "Behaviours": [{ "type": "ZombieSpawn", "order": 1 }] },
        { "typeId": "Spawn.Player", "position": "07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53", "Behaviours": [{ "type": "PartySpawn"}] },
        { "typeId": "Door", "position": "55f54395-6e94-4d1a-9694-824050f4a867" },
        
        { "typeId": "Area", "position": ["d7de841d-64a5-48b3-9662-0fe757a8950e"] },
        { "typeId": "Area", "position": ["dffe31ca-330e-404a-87f3-21555b600ede"] },
        { "typeId": "Area", "position": ["dd05ee1d-7a6e-4996-b0bc-a9acb50fe3de"] },
        { "typeId": "Area", "position": ["52aef277-2960-4974-8a29-e9e981d11c1a"] },
        { "typeId": "Area", "position": ["c3e99a3c-0a24-42af-961e-1d8be2352bea"] },
        { "typeId": "Area", "position": ["1bd502ad-cfcd-4a07-94a7-93634651a1c3"] },
        
        { "typeId": "Area", "position": ["66a0dadc-d774-4ce9-a3ec-0213e9528af6"], "Behaviours": [{ "type": "Indoor" }] },
        { "typeId": "Area", "position": ["6792b7cd-01d7-4bf7-95b4-d2876d5b4020"], "Behaviours": [{ "type": "Indoor" }, { "type": "Dark" }] },
        { "typeId": "Area", "position": ["07ea7bbc-4f23-4bf0-a5c7-c527f36c3b53"], "Behaviours": [{ "type": "Indoor" }] }
      ]
    }
    """;
    
    public static TurnForge.Engine.Entities.Board.Descriptors.BoardDescriptor GetMission01BoardDescriptor()
    {
        var (spatial, zones, _, _) = BarelyAlive.Rules.Adapter.Loaders.MissionLoader.ParseMissionString(Mission01Json);
        return new TurnForge.Engine.Entities.Board.Descriptors.BoardDescriptor(spatial, zones);
    }
}
