# Implementation Plan - FEATURE-013: Commands Workflow Engine

**Goal**: Implement the generic Workflow Engine to decouple game rules from engine orchestration, following a strict 9-phase roadmap.

## User Review Required
> [!IMPORTANT]
> **No avanzar a la siguiente fase sin tests verdes en la actual.**

## Roadmap

### **Fase 0 — Contratos** (DONE)
- [x] Interfaces base: `IWorkflow`, `INode`, `IWorkflowOrchestrator`, `WorkflowContext`
- [x] Value Objects: `WorkflowId`, `NodeId`, `ReactionId`, `WorkflowStatus`, `ValidationResult`
- [x] `IInputActionResult`
- [x] `IReaction` + `ReactionResult`
- [x] `WorkflowExecutionResult`

### **Fase 1 — Orchestrator Core** (DONE)
- [x] Implementar `WorkflowOrchestrator` básico
- [x] Ejecución lineal de nodos
- [x] Llamada a `Node.Validate`
- [x] Soporte de `Completed`, `Cancelled`, `Suspended`
- [x] Protección contra loops infinitos
- [x] Trazado de transiciones (`NodeTransition`)
- [x] **Sin** reactions, events, decisions
- [x] **Verification**: `WorkflowOrchestratorTests` (Linear execution, Cancel)

### **Fase 2 — Input & Suspension** (DONE)
- [x] Detección de nodos que requieren input (`IAcceptsInput`)
- [x] Suspensión automática si falta `IInputActionResult`
- [x] API de reanudación: `Resume(IInputActionResult input)`
- [x] Validación de input por tipo (Implícito en genérico `IAcceptsInput<T>`)
- [x] Reintento seguro del nodo tras reanudación
- [x] **Verification**: Tests de suspensión / reanudación

### **Fase 3 — Reactions (Rules Engine)** (DONE)
- [x] Ejecución de `IAcceptsReactions.AllowedReactions`
- [x] Pipeline: `CanReact` -> `React`
- [x] Soporte de modificación de input (Auto-Resolution implemented)
- [x] Orden determinista de reactions (Implicit in Collection)
- [x] Prevención de loops de reactions (Linear pass per node)
- [x] **Verification**: Tests de reglas simples (reroll, trigger) -> Added Tests for Suspension/ModifiedInput

### **Fase 4 — Nested Workflows** (DONE)
- [x] Soporte para `ReactionResult.WithNestedWorkflow`
- [x] Ejecución depth-first del workflow anidado
- [x] Propagación de cancelación / suspensión
- [x] Reanudación correcta del workflow padre
- [x] Protección contra recursión infinita
- [x] **Verification**: `WorkflowOrchestratorTests` (Nested execution, Stack management)

### **Fase 5 — Events & Decision History** (DONE)
- [x] Introducir `IWorkflowEvent`
- [x] Almacenamiento de eventos en `WorkflowContext`
- [x] Emisión de eventos desde nodos (EndNode & Intermediate)
- [x] Consumo de eventos por reactions (Event Processing Loop)
- [x] Política de limpieza de eventos
- [x] **Verification**: Tests de eventos pasivos (trap, aura) y triggers intermedios

### **Fase 6 — Atomic State Application & Projection** (DONE)
**Goal**: Ensure Nodes make decisions based on the *projected* future state (Real State + Pending Decisions) without mutating the real state until completion.

- [x] **Design & Interfaces**
    - [x] Find/Define `IGameState` and `IStateProjector` concept
    - [x] Add `IDecision.Apply(IGameState)` method
    - [x] Create `ProjectedGameState` wrapper
- [x] **Implementation**
    - [x] Implement `ProjectionService` (applies context.Decisions to State View)
    - [x] Integrate Projector into `WorkflowContext`
    - [x] Update `WorkflowOrchestrator` to apply decisions on Success (Atomic Commit)
- [x] **Verification**
    - [x] Test: Node A modifies state (virtually), Node B reads modified state
    - [x] Test: Workflow Cancelled -> Real state untouched
    - [x] Test: Workflow Completed -> Real state updated

### Phase 7: FSM Integration <!-- id: 7 -->
- [ ] **Infrastructure**
    - [ ] Extract `IWorkflowOrchestrator` interface from `WorkflowOrchestrator`.
    - [ ] Inject `IWorkflowOrchestrator` into `GameEngineRuntime`.
- [ ] **FSM Extensions**
    - [ ] Update `NodeExecutionResult` to include `IWorkflow? WorkflowToLaunch` and `WorkflowContext? InitialContext`.
    - [ ] Update `FsmStepResult` to include `IWorkflow? WorkflowToLaunch` etc.
    - [ ] Update `FsmController` to propagate Workflow requests from Node to Runtime.
- [ ] **Runtime Integration**
    - [ ] Add `_activeWorkflow` and `_activeWorkflowContext` state to `GameEngineRuntime`.
    - [ ] Define `WorkflowInputCommand` (wrapper for `IInputActionResult`).
    - [ ] Modify `GameEngineRuntime.ExecuteCommand`:
        - [ ] Check if `_activeWorkflow` is `Suspended`.
        - [ ] If Suspended & Command is `WorkflowInputCommand`: Resume Workflow.
        - [ ] If Not Suspended: Normal FSM flow.
        - [ ] If FSM returns `WorkflowToLaunch`: Start Workflow.
        - [ ] Handle Workflow Completion: Apply Decisions (Atomic Commit), Clear active state.
- [ ] **Verification**
    - [ ] Test: FSM triggers combat workflow -> Runtime enters suspended state.
    - [ ] Test: User sends `WorkflowInputCommand` -> Workflow resumes -> Workflow Completes -> Decisions applied.

### **Fase 8 — Hardening**
- [ ] Logging estructurado
- [ ] Instrumentación
