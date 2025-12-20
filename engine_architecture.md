
# Arquitectura del Motor TurnForge

Este documento detalla la arquitectura técnica del motor `TurnForge.Engine`, sus interfaces principales y cómo extenderlo mediante Estrategias y Comportamientos (Behaviours).

## 1. Visión General (High-Level Architecture)

`TurnForge.Engine` sigue una arquitectura basada en **Comandos (Command Pattern)** y **Eventos (Event Sourcing / Domain Events)**, con un estado inmutable (`GameState`) gestionado centralmente.

### Componentes Principales

*   **TurnForge (Facade)**: Punto de entrada principal. Expone:
    *   `Runtime`: Para la ejecución del juego y envío de comandos.
    *   `GameCatalog`: Para el registro de definiciones (Agentes, Props, etc.).
*   **GameEngineRuntime**: Implementación de `IGameEngine`. Orquesta el flujo del juego recibiendo `ICommand` y despachándolos a través de un `CommandBus`.
*   **CommandBus**: Enruta los comandos a sus respectivos `ICommandHandler`.
*   **GameState**: Representación inmutable del estado del juego en un momento dado. Contiene todos los Agentes, Props, y el estado del tablero.
*   **IGameRepository**: Interfaz para persistencia (Cargar/Guardar `GameState`).

## 2. Interfaces Core

### IGameEngine
Interfaz principal para interactuar con el juego en ejecución.
```csharp
public interface IGameEngine
{
    CommandResult Send(ICommand command);
    void Subscribe(Action<IGameEffect> handler);
}
```

### ICommand & ICommandHandler
Todo cambio en el juego se realiza mediante comandos.
```csharp
public interface ICommand { }

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    CommandResult Handle(TCommand command);
}
```

### IGameEffect
Eventos emitidos como resultado de procesar comandos (ej. `AgentSpawnedEffect`). Los clientes (como Godot o una UI) se suscriben a estos efectos para actualizar la vista.

## 3. Extensibilidad y Personalización

### Estrategias (Strategies)
El motor utiliza el patrón Estrategia para lógica configurable, especialmente en el spawneo.

*   **IAgentSpawnStrategy**: Decide dónde y qué Agentes (jugadores/enemigos) aparecen.
    *   `Decide(AgentSpawnContext context)`: Retorna una lista de `AgentSpawnDecision`.
*   **IAgentNpcSpawnStrategy**: Estrategia específica para NPCs, desacoplada de la entrada del jugador (`AgentDescriptor`).
    *   `Decide(AgentNpcSpawnContext context)`: Retorna decisiones basadas puramente en el `GameState`.

### Comportamientos (Behaviours) y Agentes
Los Agentes (`Agent`) son las entidades activas. Su comportamiento se define mediante:

1.  **AgentDefinition**: Datos bases (Salud, Movimiento, AP).
2.  **IActorBehaviour**: Lógica modular adjunta a un agente.

#### IActorBehaviour
Interfaz para definir capacidades o lógicas específicas (ej. `ZombieSpawnBehaviour`, `ExplosiveBehaviour`).
Los comportamientos pueden ser:
*   **Pasivos**: Datos o marcadores.
*   **Activos**: Intervienen en el cálculo de acciones o reaccionan a eventos (dependiendo de la implementación específica del handler).

### Flujo de Personalización
Para crear un juego personalizado sobre TurnForge:
1.  **Definir Agentes**: Crear `AgentDefinition` y registrarlos en `GameCatalog`.
2.  **Configurar Estrategias**: Implementar `IAgentSpawnStrategy` / `IAgentNpcSpawnStrategy` para definir las reglas de aparición.
3.  **Implementar Behaviours**: Crear clases que implementen `IActorBehaviour` para lógicas especiales y asignarlas a las definiciones de agentes.

## 4. Integración con Godot (BarelyAlive.Godot)
El adaptador de Godot (`BarelyAlive.Godot`) actúa como la capa de presentación.
*   Se suscribe a `IGameEffect` del motor.
*   Traduce efectos a nodos/escenas de Godot visuales.
*   Envía input del usuario como `ICommand` al motor.

---
*Documento generado automáticamente por Antigravity*
