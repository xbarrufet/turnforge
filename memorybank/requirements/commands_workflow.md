casos de uso

Cada nodo necesita unos datos para poder hacer su tarabajo (inputActioResult), puede ser resultados de tiradas, seleecciones
Antes de enviar esta informacion el usuario puede enviar otros commands cambiaran el estado del juego y estos afectaran al nodo que estaba en ejecucion, a su vez puede generar eventos que afectaran al estado del juego

               <----commmand
   --->Workflow1
              ----> Nodo 1   
                      <---- inputActionResult
              -----> Nodo 2
                     <------ command
                        ----> Workflow2
                              ----> Nodo 3
                                <---- inputActionResult
                     <------- Esta
                    <---- inputActionResult
               <---- estdado













# Commands Workflow – Requirements (Revised)

## 1. Objetivo

El objetivo del sistema de *Commands Workflow* es proporcionar un **mecanismo genérico, extensible y desacoplado** para la ejecución de comandos de juego complejos, modelados como **secuencias explícitas de nodos**, donde:

* el engine **orquesta el flujo**,
* el juego **define las reglas**,
* y **ningún cambio permanente** se aplica al estado del juego hasta el final del workflow.

---

## 2. Principios fundamentales

1. El engine **no conoce reglas del juego**.
2. El engine **no genera resultados aleatorios ni decisiones de usuario**.
3. Toda lógica de reglas se expresa mediante **Reactions**.
4. Los **Nodes son estructurales**, no lógicos.
5. Todo efecto persistente se expresa como **Decision**.
6. El workflow es una **unidad transaccional lógica**.

---

## 3. Estructura general del Workflow

Un `Command` se ejecuta como un **Workflow**, definido como una secuencia lineal de `Nodes`.

```text
Node₀ → Node₁ → … → Nodeₙ (EndNode)
```

El `WorkflowOrchestrator`:

* mantiene el nodo activo,
* valida la continuidad del flujo,
* ejecuta reacciones permitidas,
* avanza o cancela el workflow.

---

## 4. Contextos y datos

### 4.1 WorkflowContext

Contiene **únicamente datos temporales** necesarios para la ejecución del workflow.

* Es mutable durante el workflow.
* No representa estado persistente del juego.
* Vive aislado del `GameState` real.

### 4.2 GameState

* Estado persistente del juego.
* **Nunca se modifica directamente** durante el workflow.
* Solo se ve afectado por `Decisions` al finalizar correctamente.

---

## 5. InputActionResult

El engine **no sabe generar inputs**.

Un `InputActionResult` es:

* un resultado **crudo** generado externamente (tiradas, selecciones, cartas, UI),
* opaco para el engine,
* específico del juego.

Ejemplos:

* resultado de pool de dados,
* resultado enfrentado,
* selección de una unidad,
* carta robada.

El engine **no interpreta** este resultado.

---

## 6. ActionResult

Un `ActionResult` es un **resultado interpretado**, derivado del `InputActionResult` por reglas del juego.

Características:

* Tiene un booleano raíz (`IsSuccess`).
* Puede incluir magnitud (`Total`).
* Puede incluir desgloses semánticos (`Breakdowns`).

El engine **transporta** `ActionResult`, pero **no decide cómo se produce**.

---

## 7. Nodes

### 7.1 Responsabilidad de un Node

Un `Node` **no implementa reglas del juego**.

Sus únicas responsabilidades son:

1. **Declarar qué Reactions son válidas** en ese punto del workflow.
2. **Validar** si el workflow puede continuar.
3. **Aplicar la transición mínima**:

   * traducir el input al `WorkflowContext`,
   * avanzar al siguiente nodo.

Un Node:

* no decide rerolls,
* no aplica costes,
* no interpreta críticos,
* no modifica el `GameState`.

---

### 7.2 Validación de Node

Cada Node expone una validación que se ejecuta:

* al entrar en el nodo,
* después de cada reacción.

La validación puede producir:

* continuar,
* cancelar el workflow,
* redirigir a otro nodo,
* suspender la ejecución.

Esto permite:

* fugas (teletransporte),
* interrupciones limpias,
* cambios de flujo controlados.

---

## 8. Reactions

Una `Reaction` representa **cualquier regla del juego** que:

* responde al estado actual del workflow,
* puede modificar el `WorkflowContext`,
* puede modificar el `InputActionResult`,
* puede consumir recursos,
* puede lanzar workflows anidados.

### 8.1 Principio clave

> **Toda modificación del input debida a reglas del juego es siempre una Reaction.**

Esto incluye:

* rerolls gratuitos,
* rerolls con coste,
* modificadores automáticos,
* efectos condicionados.

El coste (AP, recursos, cooldown) es un **detalle de la reacción**, no una categoría distinta.

---

## 9. Cancelación y atomicidad

El workflow:

* no aplica cambios persistentes durante su ejecución,
* acumula únicamente información temporal.

Si el workflow se cancela:

* se descartan todos los efectos,
* el `GameState` permanece intacto.

No se requieren copias completas del estado:

* la atomicidad se garantiza mediante `Decisions`.

---

## 10. EndNode y Decisions

El último nodo del workflow es siempre un `EndNode`.

Un `EndNode`:

* no acepta reacciones,
* no modifica contexto ni estado,
* **genera una lista de `Decisions`**.

Las `Decisions`:

* describen cambios persistentes,
* se aplican al `GameState` solo si el workflow finaliza correctamente.

Ejemplos:

* aplicar daño,
* mover unidades,
* eliminar entidades,
* consumir recursos persistentes.

---

## 11. Tipos de Node (por capacidades)

Los Nodes no se diferencian por herencia rígida, sino por **capacidades**:

* Nodes que aceptan input
* Nodes que aceptan reacciones
* Nodes que producen decisiones

Ejemplos conceptuales:

* SystemNode: sin input, sin reacciones
* WorkflowNode: con input y reacciones
* SelectionNode: input de selección
* EndNode: produce decisiones

---

## 12. Alcance del Engine

El engine:

* orquesta workflows,
* valida estructura,
* ejecuta reacciones,
* aplica decisiones finales.

El engine **no**:

* conoce dados,
* conoce críticos,
* conoce reglas de juego,
* interpreta inputs.

---

## 13. Conclusión

Este modelo define un **workflow declarativo, determinista y extensible**, adecuado para:

* wargames tácticos complejos,
* reglas altamente reactivas,
* sistemas con múltiples capas de excepciones,

manteniendo el engine:

* limpio,
* testeable,
* reutilizable,
* y desacoplado del juego concreto.

---

// ============================================================================
// TurnForge.Engine – Commands Workflow Contracts
// Single-file reference interfaces and value objects
// ============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace TurnForge.Engine.Workflow
{
    // ------------------------------------------------------------------------
    // Identifiers (Value Objects)
    // ------------------------------------------------------------------------

    public readonly record struct WorkflowId(string Value);
    public readonly record struct NodeId(string Value);
    public readonly record struct ReactionId(string Value);

    // ------------------------------------------------------------------------
    // Workflow
    // ------------------------------------------------------------------------

    public interface IWorkflow
    {
        WorkflowId Id { get; }
        INode StartNode { get; }
    }

    // ------------------------------------------------------------------------
    // Workflow Orchestrator
    // ------------------------------------------------------------------------

    public interface IWorkflowOrchestrator
    {
        WorkflowExecutionResult Execute(
            IWorkflow workflow,
            WorkflowContext context,
            GameStateSnapshot gameState);
    }

    // ------------------------------------------------------------------------
    // Nodes
    // ------------------------------------------------------------------------

    public interface INode
    {
        NodeId Id { get; }

        ValidationResult Validate(WorkflowContext context);

        INode? NextNode { get; }
    }

    public interface IAcceptsInput<in TInput>
        where TInput : IInputActionResult
    {
        void MoveForward(
            WorkflowContext context,
            TInput input);
    }

    public interface IAcceptsReactions
    {
        IReadOnlyCollection<IReaction> AllowedReactions { get; }
    }

    public interface IProducesDecisions
    {
        IReadOnlyList<IDecision> BuildDecisions(
            WorkflowContext context);
    }

    // ------------------------------------------------------------------------
    // Reactions
    // ------------------------------------------------------------------------

    public interface IReaction
    {
        ReactionId Id { get; }

        bool CanReact(WorkflowContext context);

        ReactionResult React(
            WorkflowContext context,
            IInputActionResult? input);
    }

    public sealed class ReactionResult
    {
        public WorkflowContext Context { get; }
        public IInputActionResult? ModifiedInput { get; }
        public IWorkflow? NestedWorkflow { get; }

        private ReactionResult(
            WorkflowContext context,
            IInputActionResult? modifiedInput,
            IWorkflow? nestedWorkflow)
        {
            Context = context;
            ModifiedInput = modifiedInput;
            NestedWorkflow = nestedWorkflow;
        }

        public static ReactionResult NoChange(WorkflowContext context)
            => new(context, null, null);

        public static ReactionResult WithModifiedInput(
            WorkflowContext context,
            IInputActionResult modifiedInput)
            => new(context, modifiedInput, null);

        public static ReactionResult WithNestedWorkflow(
            WorkflowContext context,
            IWorkflow nestedWorkflow)
            => new(context, null, nestedWorkflow);
    }

    // ------------------------------------------------------------------------
    // Inputs
    // ------------------------------------------------------------------------

    public interface IInputActionResult
    {
    }

    // ------------------------------------------------------------------------
    // Interpreted Results
    // ------------------------------------------------------------------------

    public sealed class ActionResult
    {
        public bool IsSuccess { get; }
        public int Total { get; }
        public IReadOnlyList<ResultBreakdown> Breakdowns { get; }

        private ActionResult(
            bool isSuccess,
            int total,
            IReadOnlyList<ResultBreakdown> breakdowns)
        {
            IsSuccess = isSuccess;
            Total = total;
            Breakdowns = breakdowns;
        }

        public static ActionResult Success(
            int total = 1,
            IEnumerable<ResultBreakdown>? breakdowns = null)
            => new(
                true,
                total,
                breakdowns?.ToList() ?? Array.Empty<ResultBreakdown>());

        public static ActionResult Failure(
            int total = 0,
            IEnumerable<ResultBreakdown>? breakdowns = null)
            => new(
                false,
                total,
                breakdowns?.ToList() ?? Array.Empty<ResultBreakdown>());
    }

    public sealed class ResultBreakdown
    {
        public string Kind { get; }
        public int Amount { get; }

        public ResultBreakdown(string kind, int amount)
        {
            if (string.IsNullOrWhiteSpace(kind))
                throw new ArgumentException(nameof(kind));

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            Kind = kind;
            Amount = amount;
        }
    }

    // ------------------------------------------------------------------------
    // Validation
    // ------------------------------------------------------------------------

    public abstract record ValidationResult
    {
        public sealed record Ok : ValidationResult;
        public sealed record Cancel : ValidationResult;
        public sealed record Redirect(NodeId TargetNode) : ValidationResult;
        public sealed record Suspend : ValidationResult;

        public static readonly ValidationResult OkResult = new Ok();
        public static readonly ValidationResult CancelResult = new Cancel();
    }

    // ------------------------------------------------------------------------
    // Decisions
    // ------------------------------------------------------------------------

    public interface IDecision
    {
        void Apply(GameState gameState);
    }

    // ------------------------------------------------------------------------
    // Contexts and Game State
    // ------------------------------------------------------------------------

    public abstract class WorkflowContext
    {
    }

    public abstract class GameStateSnapshot
    {
    }

    public abstract class GameState
    {
    }

    // ------------------------------------------------------------------------
    // Workflow Execution Result
    // ------------------------------------------------------------------------

    public sealed class WorkflowExecutionResult
    {
        public bool IsCancelled { get; }
        public IReadOnlyList<IDecision> Decisions { get; }

        private WorkflowExecutionResult(
            bool isCancelled,
            IReadOnlyList<IDecision> decisions)
        {
            IsCancelled = isCancelled;
            Decisions = decisions;
        }

        public static WorkflowExecutionResult Cancelled()
            => new(true, Array.Empty<IDecision>());

        public static WorkflowExecutionResult Completed(
            IReadOnlyList<IDecision> decisions)
            => new(false, decisions);
    }
}



El **SelectionNode** encaja bien en tu arquitectura, pero —igual que el `DecisionNode`— **solo si se define como un nodo estructural**, no como un nodo “inteligente”.
Bien definido, **refuerza el modelo**; mal definido, **lo rompe**.

Voy directo a lo esencial.

---

## 1. Qué es realmente un `SelectionNode`

No es un nodo “especial”.
Es simplemente un nodo cuyo **input no es aleatorio**, sino **una elección explícita**.

Definición correcta:

> **Un SelectionNode es un Node que acepta como `InputActionResult` una selección hecha por el jugador o el sistema, sin interpretar su significado.**

No decide:

* si la selección es buena,
* qué implica en reglas,
* qué efectos produce.

Solo **recibe la selección y avanza**.

---

## 2. Qué problema resuelve (y por qué sí tiene sentido)

Hay flujos donde el siguiente paso **no depende de una tirada**, sino de una **elección**:

Ejemplos reales:

* elegir objetivo secundario,
* elegir qué unidad recibe el efecto,
* elegir perdedor de un enfrentado,
* elegir modo de resolución.

Forzar esto a pasar por:

* un dado,
* o un ActionResult booleano,

es artificial.

El `SelectionNode` **normaliza la elección como input**, igual que normalizas una tirada.

---

## 3. Riesgo principal: convertirlo en lógica de reglas

El peligro es que el `SelectionNode`:

* valide reglas de selección,
* filtre opciones,
* aplique efectos,
* decida consecuencias.

Eso **no es su trabajo**.

### Regla de oro

> **El SelectionNode no decide qué se puede seleccionar; solo recibe lo que se ha seleccionado.**

La validez:

* se comprueba en `Validate`,
* o en Reactions,
* o antes (UI / juego).

---

## 4. Encaje con tus contratos actuales

Tu diseño **ya lo soporta** sin cambios.

Formalmente, es solo esto:

```csharp
public sealed class SelectionResult<T> : IInputActionResult
{
    public T Selected { get; }

    public SelectionResult(T selected)
    {
        Selected = selected;
    }
}
```

Y el nodo:

```csharp
public sealed class SelectionNode<T> :
    INode,
    IAcceptsInput<SelectionResult<T>>,
    IAcceptsReactions
{
    public NodeId Id { get; }

    public INode? NextNode { get; }

    public IReadOnlyCollection<IReaction> AllowedReactions { get; }

    public ValidationResult Validate(WorkflowContext context)
        => ValidationResult.OkResult;

    public void MoveForward(
        WorkflowContext context,
        SelectionResult<T> input)
    {
        // Traducción mínima:
        // context.SelectedTarget = input.Selected;
    }
}
```

Nada más.

---

## 5. SelectionNode + Reactions (punto importante)

Un `SelectionNode` **sí puede aceptar reacciones**, y esto es clave.

Ejemplos:

* reacción que cambia la lista de opciones,
* reacción que fuerza una selección automática,
* reacción que cancela el workflow si no hay opciones válidas.

Pero:

* la reacción **no vive en el nodo**,
* el nodo solo declara que se permiten.

---

## 6. Relación con `DecisionNode`

No se pisan, se complementan.

Flujo típico:

```text
SelectionNode (elige objetivo)
   ↓
WorkflowNode (resuelve efectos)
   ↓
DecisionNode (genera decisiones)
```

El `SelectionNode` decide **“sobre quién”**,
el `DecisionNode` decide **“qué pasa finalmente”**.

Separación limpia.

---

## 7. Cuándo NO usar un SelectionNode

No lo uses si:

* la “selección” es en realidad una regla automática,
* no hay intervención del jugador o sistema externo,
* el valor puede derivarse del contexto sin input.

En esos casos:

* usa un SystemNode,
* o una Reaction.

---

## 8. Veredicto final

**Mi evaluación:**

* ✔ El `SelectionNode` es conceptualmente correcto.
* ✔ Encaja perfectamente con tu arquitectura.
* ✔ Refuerza la separación input / reglas / efectos.
* ❌ No debe validar reglas complejas.
* ❌ No debe aplicar consecuencias.
* ❌ No debe “decidir”.

Trátalo como:

> **un punto de entrada de decisión externa**,
> exactamente igual que una tirada, pero sin aleatoriedad.

Si quieres, el siguiente paso natural sería:

* añadir `SelectionResult<T>` al fichero de contratos,
* o ver un **ejemplo 40K/MESBG donde el perdedor se selecciona explícitamente** para validar el patrón.


A continuación tienes **el documento de requirements de EVENTOS**, redactado como **Fase 2 del desarrollo**, coherente con todo lo definido previamente y listo para añadirse tal cual a la documentación del proyecto.

No introduce arquitectura nueva: **formaliza** lo ya acordado.

---

# Phase 2 – Workflow Events Requirements

## 1. Objetivo

Definir un **sistema de eventos interno al workflow** que permita:

* desacoplar **hechos ocurridos** de **reglas del juego**,
* habilitar **reacciones contextuales** sin acoplar nodos a lógica de reglas,
* mantener el engine **agnóstico del dominio del juego**,
* preservar la **atomicidad y determinismo** del workflow.

Los eventos **no sustituyen** Commands, FSM ni Decisions.
Son un **mecanismo interno de señalización**.

---

## 2. Principios fundamentales

1. Un **Event describe un hecho**, no una consecuencia.
2. Un Event **no ejecuta lógica**.
3. Un Event **no muta GameState**.
4. Un Event **no inicia workflows ni commands**.
5. Los Events son **locales al workflow**.
6. Toda interpretación de un Event se realiza mediante **Reactions**.
7. Los Events son **transitorios y no persistentes**.

---

## 3. Definición de Event

### 3.1 Naturaleza

Un `WorkflowEvent` representa:

> Algo que **ha ocurrido objetivamente** durante la ejecución de un workflow.

Ejemplos válidos:

* una unidad entra en un tile,
* se completa un movimiento,
* se consume un recurso temporal,
* se alcanza un nodo,
* se resuelve una tirada.

Ejemplos NO válidos:

* “se activa una trampa”
* “se hace daño”
* “se dispara una habilidad”

---

### 3.2 Contrato base

El engine define el contrato base del evento, pero **no sus implementaciones concretas**.

```csharp
public interface IWorkflowEvent
{
}
```

El contenido semántico del evento es **100% responsabilidad del juego**.

---

## 4. Alcance y ciclo de vida

### 4.1 Alcance

* Los Events existen **solo dentro del workflow activo**.
* No son visibles para:

  * la FSM,
  * otros workflows,
  * el GameState persistente.

---

### 4.2 Ciclo de vida

1. El Node ejecuta su transición mínima.
2. El Node añade uno o más Events al `WorkflowContext`.
3. El Orchestrator evalúa Reactions válidas.
4. Las Reactions consultan los Events.
5. Los Events pueden limpiarse antes de avanzar al siguiente Node.

Los Events:

* **no sobreviven** al workflow,
* **no se serializan**,
* **no se propagan globalmente**.

---

## 5. Almacenamiento de Events

### 5.1 Ubicación

Los Events se almacenan en el `WorkflowContext`.

```csharp
public abstract class WorkflowContext
{
    IReadOnlyList<IWorkflowEvent> Events { get; }
}
```

Características:

* colección mutable durante el workflow,
* lectura libre por Reactions,
* escritura exclusiva del workflow (Nodes).

---

### 5.2 Gestión

El engine debe permitir:

* añadir eventos,
* consultar eventos,
* limpiar eventos entre nodos si el juego lo requiere.

La política de limpieza:

* es **determinista**,
* es **responsabilidad del orchestrator**,
* puede ser:

  * por nodo,
  * por fase,
  * manual.

---

## 6. Quién lanza Events

### 6.1 Responsable

> **Los Events solo pueden ser lanzados por el Workflow, normalmente desde los Nodes.**

Un Node:

* lanza Events como parte de su transición mínima,
* **no interpreta** el significado del Event,
* **no consulta** GameState para decidir si lanzar un Event.

Ejemplo válido:

```
TileEnteredEvent
```

Ejemplo inválido:

```
TrapTriggeredEvent
```

---

### 6.2 Restricciones

* Las Reactions **no deben lanzar Events base**.
* El GameState **no lanza Events**.
* La FSM **no lanza Events**.
* Los Commands **no lanzan Events**.

Esto evita:

* duplicidad semántica,
* reglas implícitas,
* efectos laterales invisibles.

---

## 7. Quién recibe Events

### 7.1 Consumidores

Los Events son consumidos por:

* **Reactions válidas en el Node activo**.

Las Reactions:

* consultan Events en `CanReact`,
* interpretan Events usando:

  * reglas del juego,
  * `GameStateSnapshot`,
  * `WorkflowContext`.

---

### 7.2 No existe un EventBus global

El sistema de Events:

* **no es pub/sub**,
* **no es observable global**,
* **no permite subscripciones persistentes**.

Esto es deliberado.

---

## 8. Relación Event → Reaction → Decision

El pipeline obligatorio es:

```text
Event (hecho)
 → Reaction (regla)
   → Decision (efecto persistente)
```

Reglas clave:

* Un Event **nunca produce directamente una Decision**.
* Una Reaction **puede producir cero o más Decisions**.
* Una Decision **es el único mecanismo que muta GameState**.

---

## 9. Relación con GameState

### 9.1 Conocimiento del mundo

El Event **no conoce el mundo**.

Ejemplo:

```
TileEnteredEvent(tileId)
```

La Reaction:

* consulta el `GameStateSnapshot`:

  * si hay trampa,
  * si la unidad es inmune,
  * si la trampa está activa.

El Event **no expresa condiciones**, solo hechos.

---

### 9.2 Acceso permitido

Las Reactions:

* pueden leer `GameStateSnapshot`,
* **no pueden mutar** `GameState`.

---

## 10. Relación con InputActionResult

* Los Events **no son inputs**.
* Los Inputs:

  * vienen del exterior (UI, IA, red),
  * representan decisiones o azar.

Un Event puede:

* provocar que una Reaction solicite un Input,
* suspender el workflow hasta recibirlo.

---

## 11. Errores de diseño explícitamente prohibidos

❌ Events que ejecutan lógica
❌ Events que mutan estado
❌ Events que disparan Commands
❌ Events globales
❌ Nodes que interpretan Events
❌ GameState reaccionando a Events

---

## 12. Ejemplo completo (referencia)

```text
MoveNode
 └─ emite TileEnteredEvent

TrapReaction
 └─ detecta TileEnteredEvent
 └─ consulta GameStateSnapshot
 └─ genera ApplyDamageDecision(2)

EndNode
 └─ retorna Decisions
```

---

## 13. Alcance del Engine vs Juego

### Engine

* contrato `IWorkflowEvent`
* almacenamiento y ciclo de vida
* orquestación de consumo

### Juego

* definición de eventos concretos
* semántica del evento
* reglas que reaccionan al evento

---

## 14. Conclusión

El sistema de Events:

* refuerza la separación Node / Rule / Effect,
* evita lógica oculta,
* mantiene el engine genérico,
* permite reglas complejas sin acoplamiento,
* es consistente con FSM y Workflow.

Este sistema **no es opcional** para workflows reactivos complejos:
es el mecanismo formal que evita que las reglas “se cuelen” en los nodos.

---

Si quieres, el siguiente paso natural sería:

* integrar este documento como **ADR de Phase 2**,
* o definir **tests de conformidad** para validar que un workflow respeta estas reglas.
