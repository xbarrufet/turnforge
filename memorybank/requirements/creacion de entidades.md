

La diferencia entre els diferentes tipus d'entitatas son els Tratits que porten

GameEntity (basic trait container)

Actor: (base for agents and prods)
- PositionTrait
- HealthTrait

Agent: (actors that can take actions)
- ActionTrait
- InventoryTrait

Prod: (actors that can be interacted with)
- ActionableTrait

Item: (Game Items)
-
Weapon: (Game Items that can be used to attack)
- DamageTrait


Creacio de Traits
- Per crear un Trait l'usuari extend BaseTrait si es nomes un valor inmutable o extend de BaseComponentTrait si es un valor mutable, en aquest cas ha de definiir el Trait i el Component tenint el trait como a constructor

Ex: SpawnOrderTrait:BaseTtrait {
    int SpawnOrder {get; set;}
}

Ex: AdrenalineTrait:BaseComponentTrait {
    int InitialAdrenaline {get; set;}
}
AdrenalineComponent:BaseComponent {
    int CurrentAdrenaline {get; set;}
    int MaxAdrenaline {get; set;}
    public AdrenalineComponent(AdrenalineTrait trait) {
        CurrentAdrenaline = trait.InitialAdrenaline;
    }
}
    

Creacio de Entitats del Joc
- Usuari crea definicions de les noves entitats 


Usuari crea Definicions, extenen algun d'aquests tipus i afegint els traits que necessit
i posant al constuctor els valors inicials dels traits (els q necessitis). Els traits base de cada tipus tenen el constuctor
Ex: 
    [EntityTraits([AdrenalieTrait])] //lista de traits que afegeix
    public class SurvivorDefinition:AgentDefinition(string definitionId, string category, int initialAdrenaline  ):base(definitionId, category) {}

Creacio de Entitats tipus (instanciacio de definicions) 
EX:
     var mike = new SurvivorDefinition("mike", "survivor", 0);

Com creo entitats particulars (Spawns)
Dins del engine tenim la tupla Definition->Descriptor->Entity
Entity es com es gestionen les instancies internament no fa falta exposar-les al developer, ell pot treballar a nivel de descriptor com si fossin les seves entitats base

Ex:
    [DefinitionType(typeof(SurvivorDefinition))]
    class Survivor:BaseAgent {
    }

    per crer una entitat s'ha de fer servir el command spawn
    public sealed record SpawnRequest(
        string DefinitionId,  // Required: entity definition ID to spawn
        int Count = 1,       // Optional: number of entities to spawn (batch spawn)
        List<IBaseTraits> traitsToOverride; 
);
Nota: Position no es un camp del spawnrequest perque  Item no en te, ha de anar informat traitsToOverride.

    
## 🧠 Rationale & Architectural Decisions

### 1. Separation of Domain vs Engine (Clean Architecture)
*   **Problem:** Inheriting directly from `GameEntity` (Engine) exposes too much complexity (Components, Dirty Flags) to the User Domain.
*   **Decision:** User-facing entities (`Survivor`, `Zombie`) will inherit from lightweight **Descriptors** (e.g., `BaseAgent`).
*   **Benefit:** The user works with clean, data-only classes. The Engine converts these into complex `GameEntity` instances internally.

### 2. Trait-First Architecture
*   **Concept:** "Composition over Inheritance". An entity is defined strictly by the Traits it carries.
*   **Hierarchy:**
    *   `BaseTrait`: Constant/Immutable data (e.g., `SpawnOrder`).
    *   `BaseComponentTrait`: Mutable data that requires a generic `Component` to manage state (e.g., `VitalityTrait` -> `HealthComponent`).

### 3. The "Runtime Trait Aggregator"
*   **Role of Descriptor:** The Descriptor acts as the recipe for the final entity.
*   **Flow:** Definition Traits + Override Traits (from Request) = **Final Entity Traits**.
*   **Mapping:** The Factory iterates this final list and instantiates the necessary components using the `TraitInitializationService`.

### 4. Taxonomy
*   **Definitions:** Static blueprints (`SurvivorDefinition`).
*   **Descriptors:** Instance recipes (`AgentDescriptor`/`BaseAgent`).
*   **Entities:** Runtime containers (`GameEntity`).

