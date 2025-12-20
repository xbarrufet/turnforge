---

# 📜 SPEC UNIFICADA: TurnForge Orchestrator System

## 1. Objectiu de l'Arquitectura

L'**Orchestrator** és el "Cor Executiu" del motor. La seva missió és desacoblar la **intenció** (Commands/Strategies) de la **mutació real** (Appliers) de l'estat del joc. Garanteix que tota modificació del `GameState` sigui atòmica, ordenada i generi feedback visual cap a la UI (Godot).

---

## 2. El Motor de Temps: Decision Timing

TurnForge utilitza un sistema de **Timing Dinàmic** per gestionar quan s'han d'aplicar els canvis. Tota `IDecision` ha d'incloure aquesta estructura:

### DecisionTiming (Record)

* **`When`**: El disparador del cicle de vida. Valors obligatoris:
* `OnCommandExecutionEnd`: Execució immediata en tancar el Command.
* `OnStateStart`: Execució en entrar en un node de la FSM.
* `OnStateEnd`: Execució abans de sortir d'un node de la FSM.


* **`Phase`**: El nom del node de la FSM (ex: "InitialState", "PlayerTurn"). És `null` per a execucions immediates.
* **`Frequency`**: Valors: `Single` (s'executa i es purga) o `Permanent` (es manté al Scheduler fins que s'elimini explícitament).

---

## 3. L'Orchestrator (Dispatcher Central)

Manté un registre privat d'executors per garantir l'isolament del domini de simulació.

### Components Interns:

* **Applier Registry**: Diccionari `Type -> IApplier` que mapeja el tipus de decisió amb el seu executor.
* **Factory Registry**: Diccionari `Type -> IGameEntityFactory` per resoldre la construcció d'entitats segons el seu `Descriptor`.

### Flux de Treball (Estratègia d'Execució):

1. **Recepció de Command**: Arriba un command per a la seva execució.
2. **Validació**: Es valida que el command pugui executar-se en l'estat actual.
3. **Generació de Decisions**: S'executa el command i aquest genera una **Llista de Decisions** que s'encolen a l'Orchestrator (`Enqueue`).
4. **Trigger post-execució (FSM)**: En acabar l'execució del command, la FSM (o el CommandBus) crida a l'Orchestrator indicant la fase interna `OnCommandExecutionEnded`.
5. **Execució "Immediata"**: L'Orchestrator cerca decisions amb `Timing.When == OnCommandExecutionEnd` i les **aplica**.
   *Nota: "Immediat" no significa dins del mètode `Enqueue`, sinó en el següent trigger `OnCommandExecutionEnded`.*
6. **Persistència d'Estat**: Es genera el nou estat i es persisteix.
7. **Triggers d'Estat FSM**: En canvis d'estat de la FSM (ex: canvi de torn), es crida a l'Orchestrator amb:
   - `OnEnd` (Estat sortint)
   - `OnStart` (Estat entrant)
   Es processen les decisions programades per a aquestes fases.
8. **Gestió de Freqüència**:
   - `Single`: La decisió s'elimina del Scheduler després d'appliquer-se.
   - `Permanent`: Es manté (ex: efectes de verí) fins que un altre command l'elimini explícitament.

---

## 4. Fases Oblatòries d'Arrencada (Framework Lifecycle)

El sistema coordina l'inici del joc en dos passos crítics:

1. **Estat Inicial (Món)**: Només accepta `InitialGameCommand`. Crea el tauler i les `Props`.
2. **GameStartPhase (Actors)**: Només accepta `GameStartCommand`. Crea els `Agents` (Jugadors/NPCs).

---

## 5. Appliers i Feedback (EffectSink)

L'**Applier** és l'únic component amb permís per modificar les dades:

* **`BuildApplier`**: Instancia entitats usant la factory correcta i les registra al `GameState`.
* **`ComponentUpdateApplier`**: Modifica components i actualitza l'**`EffectsComponent`** de l'entitat (font de veritat per a la UI).
* **`EffectSink`**: Després de cada mutació, l'Applier **ha d'emetre** un esdeveniment per a Godot.

---

## 6. Llistat de Classes a Desenvolupar (Checklist)

| Fitxer / Classe | Responsabilitat |
| --- | --- |
| **`DecisionTiming`** | Record amb `When`, `Phase` i `Frequency`. |
| **`IDecision`** | Interfície base amb `Timing` i `OriginId`. |
| **`IOrchestrator`** | Interfície amb mètodes de registre i despatx. |
| **`TurnForgeOrchestrator`** | Implementació del Registry intern i la lògica de despatx. |
| **`IScheduler`** | Interfície de la cua persistent dins del `GameState`. |
| **`TurnScheduler`** | Implementació de la cua amb filtratge per `Phase` i `When`. |
| **`IApplier<T>`** | Interfície genèrica per als executors de mutacions. |
| **`BuildApplier`** | Resolució de factories i registre d'entitats. |
| **`ComponentUpdateApplier`** | Mutació de components i emissió a l'EffectSink. |
| **`IGameEntityFactory`** | Interfície base per a les factories d'entitats. |

---

## 7. Algorisme Clau: ExecuteScheduled

```csharp
public void ExecuteScheduled(string phase, string when) {
    // 1. Recupera decisions del Scheduler per context
    var toExecute = _scheduler.GetDecisions(d => d.Timing.Phase == phase && d.Timing.When == when);

    foreach (var decision in toExecute) {
        // 2. Executa via Applier registrat
        Apply(decision); 

        // 3. Gestió de Frequency
        if (decision.Timing.Frequency == "Single") {
            _scheduler.Remove(decision); 
        }
    }
}

```

---

### Directrius per a Anti-Gravity:

1. **Isolament**: No utilitzis referències a Godot; tota la sortida visual és via `IEffectSink`.
2. **Persistència**: El `TurnScheduler` ha de ser serialitzable perquè forma part del `GameState`.
3. **Seguretat**: Si l'entitat objectiu ja no existeix, descarta la decisió silenciosament.