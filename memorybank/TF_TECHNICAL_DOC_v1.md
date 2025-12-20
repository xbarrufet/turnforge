# Documentación Técnica TurnForge Engine (v1)

Este documento detalla la arquitectura, subsistemas y flujos de trabajo del motor **TurnForge**. Está diseñado para proporcionar a agentes de IA y desarrolladores un entendimiento profundo del sistema para analizar impactos, proponer mejoras e implementar nuevas funcionalidades.

---

## 1. Actores y Tipos de Actores

En TurnForge, las entidades que interactúan en el tablero se denominan genéricamente **Actores**.

### Tipos Principales
El sistema se ha unificado bajo el concepto de `Agent` para personajes con comportamiento, aunque existen especializaciones:

*   **`Agent`**: Representa cualquier unidad activa (jugadores, enemigos, NPCs). Se define mediante `AgentDefinition` y una colección de comportamientos (`IActorBehaviour`). Mantiene estado como `Health` y `Position`.
*   **`Prop`**: Objeto inanimado o interactivo (cajas, puertas). Definido por `PropDefinition`. Puede tener comportamientos, pero generalmente no posee IA compleja de movimiento.

### Creación (Factory)
La creación de actores está centralizada en `IActorFactory`.
*   **`IActorFactory`**: Interfaz para convertir descriptores (`AgentTypeId`, `PropTypeId`) en instancias vivas (`Agent`, `Prop`).
*   **`GenericActorFactory`**: Implementación estándar que utiliza un `IGameCatalog` para buscar definiciones base y ensamblar al actor.

---

## 2. Sistema de Behaviours

El comportamiento de los actores es modular y composable.

*   **Interfaz**: `IActorBehaviour` (definida en `TurnForge.Engine.Entities.Actors.Interfaces`).
*   **Uso**:
    *   Los `AgentDefinition` pueden tener comportamientos base.
    *   Al instanciar un actor (`Agent` o `Prop`), se le pueden inyectar comportamientos adicionales.
    *   Los comportamientos permiten extender la lógica sin herencia masiva (ej. `AggressiveBehaviour`, `InteractableBehaviour`).

---

## 3. Gestión de Comandos

La interacción con el motor se realiza exclusivamente a través de **Comandos** (Patrón Command).

### Flujo de Ejecución
1.  **Solicitud**: Se envía un `ICommand` al `IGameEngine`.
2.  **CommandBus**: El bus central recibe el comando.
    *   Valida contra el `IGameLoop` (reglas de turno, pausa, etc.).
    *   Resuelve el `ICommandHandler<T>` adecuado mediante inyección de dependencias.
3.  **Handler**: Ejecuta la lógica de negocio (modifica `GameState`, persiste cambios).
4.  **Resultado**: Retorna un `CommandResult` (éxito/fallo, etiquetas).
5.  **Reacción (FSM)**: Si el comando tiene éxito, el FSM puede reaccionar (ver Sección 5).

### Comandos Clave
*   **`InitGameCommand`**: Inicializa el juego completo en 3 fases: Tablero, Props y Agentes. Es el punto de entrada para arrancar una partida.

---

## 4. Estrategias

TurnForge desacopla la toma de decisiones lógicas de la ejecución mediante el patrón **Strategy**.

*   **`IAgentSpawnStrategy`**: Decide *dónde* y *qué* agentes aparecen. Recibe un contexto (`AgentSpawnContext`) con el estado actual y descriptores, retorna una lista de decisiones (`AgentSpawnDecision`).
*   **`IPropSpawnStrategy`**: Similar a la de agentes, pero para la disposición inicial de props.

Esto permite que juegos específicos (Host) inyecten lógica personalizada (ej. generación procedural, spawns fijos, oleadas) sin modificar el núcleo del engine.

---

## 5. Máquina de Estados (FSM)

El motor utiliza una FSM jerárquica para controlar el flujo macro del juego (fases, turnos, condiciones de victoria).

### Componentes
*   **`FsmNode`**: Unidad base. Puede ser:
    *   `BranchNode`: Nodo contenedor, define flujo entre hijos (Composite).
    *   `LeafNode`: Nodo hoja, contiene lógica ejecutable (`OnStart`, `OnEnd`, `OnCommandExecuted`).
*   **`FsmController`**: Gestiona el nodo activo y la navegación.
*   **`FlowNavigator`**: Lógica pura para determinar el siguiente estado en la jerarquía.

### Integración con Comandos
El `GameEngineRuntime` orquesta el FSM:
1.  Tras ejecutar un comando exitoso.
2.  Invoca `_fsmController.HandleCommand`.
3.  El nodo devuelve un `FsmStepResult` indicando si solicitó una transición (`TransitionRequested`).
4.  Si se solicita transición, el Runtime fuerza el avance (`MoveForwardRequest`) e inicia el siguiente estado.

### Inicialización FSM
El comando `InitGameCommand` etiqueta su resultado con `"StartFSM"`. El Runtime intercepta esta etiqueta para forzar el primer movimiento de la máquina de estados.

---

## 6. Configuración Básica de un Juego

Para configurar un juego usando TurnForge:

1.  **Definir Repositorio**: Implementar o usar `InMemoryGameRepository` para persistencia.
2.  **Definir Estrategias**: Implementar `IAgentSpawnStrategy` y `IPropSpawnStrategy`.
3.  **Preparar Contexto**: Crear `GameEngineContext` con lo anterior.
4.  **Construir Engine**: Usar `GameEngineFactory.Build(context)`.

### Ejemplo (Pseudocódigo)
```csharp
var context = new GameEngineContext(
    new MyRepository(),
    new MyPropStrategy(),
    new MyAgentStrategy()
);
var engine = GameEngineFactory.Build(context);
```

---

## 7. Arranque del Sistema

1.  **Bootstrapping**: El host (ej. aplicación de consola, test, Unity) configura las dependencias externas.
2.  **Factory Build**: `GameEngineFactory` registra servicios internos (`CommandBus`, `GameCatalog`, `ActionFactory`) y externos en un contenedor DI (`SimpleServiceProvider`).
3.  **Runtime Ready**: Se devuelve una instancia de `IGameEngine` (`GameEngineRuntime`) lista para recibir comandos.
4.  **Startup**:
    *   Se envía `InitGameCommand`.
    *   El Handler crea el mundo.
    *   El Runtime detecta "StartFSM".
    *   La FSM entra en el estado raíz inicial.

---

## 8. Infraestructura Adicional

*   **`IGameRepository`**: Abstracción para cargar/guardar `Game` y `GameState`. Fundamental para "Salvar Partida".
*   **`IEffectSink`**: Bus de eventos de solo salida. Permite al UI o sistemas audiovisuales reaccionar a lo que ocurre en el engine (ej. "Sonido de daño" al ocurrir `DamageEffect`).
*   **`Spatial` / `Zone`**: Sistema de tablero. `GameBoard` gestiona la geometría y navegación.
