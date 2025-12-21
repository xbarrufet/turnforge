
# Arquitectura BarelyAlive.Rules

Aquest document detalla l'estructura de fitxers i la lògica del motor de regles de **BarelyAlive**. El sistema se separa en capes estrictes per garantir que l'Engine (logic) i la UI (visualització) estiguin desacoblats.

## 📁 Estructura de Carpetes

```text
src/BarelyAlive.Rules/
├── Game/                          # El cor operacional (Runtime) (BarelyAliveGame, GameBootstrap)
├── Apis/                          # Contractes i punt d'entrada extern
│   ├── Handlers/                  # Implementació de casos d'ús (1 per Command)
│   ├── Messaging/                 # DTOs de sortida cap a la UI (GameResponse, payloads)
│   ├── ViewModels/                # Projeccions d'estat complet per a consultes
│   └── Interfaces/                # Definició del servei API (IBarelyAliveApis)
├── Core/                          # Lògica pura de negoci
│   ├── Domain/                    # El món de Zombicide (Agnòstic de l'Engine)
│   │   ├── Entities/              # Classes de domini
│   │   ├── Projectors/            # Traductors: Engine -> Messaging/ViewModels
│   │   │   ├── Handlers/          # Projectors específics (AgentSpawned, etc.)
│   │   │   └── Interfaces/        # IEffectProjector
│   │   ├── ValueObjects/          # Structs de dades bàsiques (Vector)
│   │   └── Descriptors/           # Configuració d'entrada (IDescriptor)
│   ├── Engine/                    # Extensions específiques de TurnForge
│   └── Strategies/                # Regles de negoci (Combat, IA, Spawn)
├── Adapter/                       # Infraestructura i entrada de dades
│   ├── Loaders/                   # Lectors de fitxers (JSON)
│   ├── Mappers/                   # Conversió de DTOs de fitxer a Descriptors
│   └── Dto/                       # Estructures que reflecteixen el fitxer JSON
└── Assets/                        # Fitxers de dades (Missions, Config)
```

---

## 🔄 Flux de Crida: De la UI a l'Engine

A continuació es detalla el cicle de vida complet d'una interacció amb el sistema.

### Exemple: `InitializeGame(missionJson)`

| Pas | Component | Acció | Responsabilitat |
| :--- | :--- | :--- | :--- |
| **1** | **UI (Godot/Client)** | Crida a `BarelyAliveApis.InitializeGame(json)` | Iniciar la interacció. No coneix l'Engine, només l'API. |
| **2** | **API Facade** | Delega a `InitializeGameHandler.Handle(json)` | Punt d'entrada únic. Encamina la petició al handler adequat. |
| **3** | **Handler** | 1. Parseja el JSON (via `MissionLoader`).<br>2. Crea `InitGameCommand` (Engine).<br>3. Crida `_gameEngine.ExecuteCommand()`. | Orquestració. Converteix dades externes en comandes internes de l'Engine. |
| **4** | **TurnForge Engine** | Executa la lògica: crea entitats (Board, Zones, Props, Agents). Retorna `CommandTransaction`.<br>**Effects Generats**:<br>- `BoardApplierResult` (Board creat)<br>- `PropSpawnedEffect` (Props creats)<br>- `AgentSpawnedResult` (Agents creats) | Lògica d'estat pura. No sap res de projeccions o UI. |
| **5** | **Handler** | Rep la `Transaction`. Crida a `DomainProjector.CreatePayload(transaction)`. | Punt de sincronització. Decideix que cal traduir la resposta per al client. |
| **6** | **DomainProjector** | Itera sobre `transaction.Effects`. Busca un `IEffectProjector` per a cada efecte.<br>- `PropSpawnedProjector`: S'activa.<br>- `AgentSpawnedProjector`: S'activa.<br>- *BoardApplierResult*: Ignorat (la UI ja té la definició JSON). | Dispatcher. Encamina cada efecte del motor al seu traductor visual. |
| **7** | **IEffectProjectors** | Tradueix `IGameEffect` -> `EntityBuildUpdate` (DTO). | Traducció. Converteix "S'ha creat l'ID 5 a (0,0)" en "Dibuixa un 'Survivor' a (0,0)". |
| **8** | **Handler** | Construeix `GameResponse` amb el payload generat. | Empaquetat. Prepara la resposta final estandarditzada. |
| **9** | **UI (Godot/Client)** | Rep `GameResponse`. Processa `Payload.Created` per instanciar nodes visuals (Agents i Props). El Mapa ja es dibuixa des del JSON inicial. | Renderitzat. Actualitza l'escena visual per reflectir el nou estat. |

---

## 📦 Payloads de les Crides

Aquesta secció serveix de referència per **mockejar** respostes quan es desenvolupa la UI sense l'Engine connectat.

### Objecte Base: `GameResponse`
Totes les crides retornen aquesta estructura:
```json
{
  "TransactionId": "guid-uuid-string",
  "Success": true,
  "Error": null,
  "Payload": { ... } // Veure detalls a sota
}
```

### 1. `InitializeGame` / `LoadGame`
**Payload Type**: `GameUpdatePayload`
**Descripció**: Llista que conté totes les entitats dinàmiques inicials (Agents i Props). El taulell (Mapa) no s'envia perquè és estàtic respecte al JSON de missió.

```json
{
  "Created": [
    {
      "EntityId": "101",
      "Type": "Survivor",  // Mapejat des de AgentType
      "DefinitionId": "Amy",
      "Position": { "X": 2, "Y": 5 },
      "State": {}
    },
    {
      "EntityId": "202",
      "Type": "Prop",      // Mapejat des de PropType
      "DefinitionId": "Door",
      "Position": { "X": 3, "Y": 5 },
      "State": {}
    }
  ],
  "Updated": [],
  "Events": []
}
```

### 2. `MoveAgent` (Exemple Futur)
**Payload Type**: `GameUpdatePayload`
**Descripció**: Actualització d'estat d'una entitat existent.

```json
{
  "Created": [],
  "Updated": [
    {
      "EntityId": "101",
      "Component": "Position",
      "NewValue": { "X": 3, "Y": 5 },
      "Delta": null
    },
    {
      "EntityId": "101",
      "Component": "ActionPoints",
      "NewValue": 2,
      "Delta": -1
    }
  ],
  "Events": []
}
```

---

## 🏷️ Enums i Responsabilitats

Aquests enums defineixen el vocabulari compartit entre Regles i UI.

| Enum | Ubicació | Responsabilitat | Valors Exemple |
| :--- | :--- | :--- | :--- |
| **Pending** | `Core/Domain/Entities` | (Pendent d'implementar) Definir bàndols. | `Survivor`, `Zombie`, `Neutral` |
| **Pending** | `Core/Domain/Entities` | (Pendent d'implementar) Estats de joc. | `PlayerTurn`, `EnemyTurn`, `Victory`, `Defeat` |

*(Nota: Actualment la majoria de tipus es gestionen com a strings o `ValueObjects` en el codi refactoritzat. Aquesta secció s'ampliarà a mesura que es formalitzin els Enums al Domini.)*