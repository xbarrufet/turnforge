
# ENTITIES_LIFECYCLE.md (Actualitzat v4)

## 1. Descripció General

Aquest document detalla el cicle de vida d'una `GameEntity` a TurnForge, des de la seva definició fins a la seva mutació en temps de joc. El sistema separa la **definició estàtica**, la **instanciació de dades** i la **execució de lògica** mitjançant un flux de decisions centralitzat.

## 2. Estructura de l'Entitat

Una entitat a TurnForge és un contenidor d'estat composable:

* **Id**: Identificador únic per a la localització per part dels Appliers.
* **Components**: Dades de domini (ex: `HealthComponent`, `PositionComponent`).
* **EffectsComponent**: Component d'estats actius (buffs/debuffs). És la **font de veritat per a la UI (Godot)**.

## 3. Arranque del Sistema (Framework Lifecycle)

El framework imposa dos estats inicials obligatoris abans de passar el control a la lògica de l'usuari per garantir la integritat del món:

### A. Estat Inicial (Món)

* **Objectiu**: Creació del `Game`, `GameBoard` i `Props` (objectes del mapa).
* **Comandament**: Només accepta `InitialGameCommand`.
* **Flux**: L'Orchestrator executa les `BuildDecisions` per a l'escenari.
* **Transició**: Passa automàticament a `GameStartPhase`.

### B. GameStartPhase (Actors)

* **Objectiu**: Creació dels `Agents` (jugadors i NPCs inicials).
* **Comandament**: Només accepta `GameStartCommand`.
* **Flux**: L'Orchestrator instancia les entitats dinàmiques via `IActorFactory`.
* **Transició**: Salta a l'estat inicial definit pel **Joc de l'Usuari**.

## 4. El Resultat del Comandament (CommandResult)

El `CommandResult` transporta la intenció cap a l'execució:

* **Decisions**: Col·lecció d'objectes `IDecision` (Build, Update, Effect).
* **Tags (`params string[]`)**: Etiquetes per a la coordinació (ex: `"StartFSM"`).

## 5. El Pipeline de l'Orchestrator

L'**Orchestrator** actua com a dispatcher centralitzat amb el seu propi registre de Appliers i Factories:

1. **Registre**: Mapeja `Type` de decisió contra `IApplier`.
2. **Filtrat (Timing Tags)**:
* **Immediate**: Execució directa via Applier.
* **Scheduled**: Es guarda al **Scheduler** (part del `GameState`) per a execucions futures (ex: `OnTurnStart`).



## 6. Integració amb la FSM

La FSM marca el ritme i l'Orchestrator executa:

* Quan la FSM canvia d'estat, l'Orchestrator demana al Scheduler les decisions pendents per a aquell `TimingTag` i les processa.
* El `EffectsComponent` de les entitats i el Scheduler es mantenen sincronitzats per a efectes recurrents (v2).

## 7. Feedback Visual (EffectSink)

Tots els Appliers emeten un esdeveniment a l'**EffectSink** després d'aplicar una decisió. Aquest bus és el canal únic perquè Godot executi animacions i sons de forma reactiva.

---

Això deixa l'arquitectura perfectament tancada: el framework prepara el terreny (Estat Inicial i GameStart), l'Orchestrator gestiona les mutacions i la FSM coordina els temps.

**Vols que preparem ara el codi del "Registry" intern de l'Orchestrator per veure com gestiona el `RegisterApplier` amb els diferents tipus de decisions?**