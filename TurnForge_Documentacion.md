### 2. Análisis de Arquitectura

#### Evaluación del Engine como Core Agnóstico
- El motor utiliza un diseño basado en **Command Bus** para manejar comandos de manera desacoplada.
- Los **Command Handlers** son responsables de ejecutar la lógica asociada a cada comando específico.
- Se emplea un **Event Bus** para notificar cambios de estado y permitir que otros componentes reaccionen a eventos sin acoplamiento directo.
- La arquitectura sigue principios de **Domain-Driven Design (DDD)**, con un enfoque en mantener el modelo de dominio puro y separado de las capas de infraestructura.

#### Patrones Clave en el Engine
- **Command Bus**: Centraliza el manejo de comandos, asegurando que cada comando sea procesado por su handler correspondiente.
  - Ejemplo: Un comando `MoveActorCommand` es enviado al Command Bus, que lo enruta al `MoveActorCommandHandler`.
- **Command Handlers**: Implementan la lógica específica para cada comando.
  - Ejemplo: `MoveActorCommandHandler` valida el movimiento y actualiza el estado del juego.
- **Event Bus**: Publica eventos generados por el motor, como `ActorMovedEvent`, para que otros sistemas (como la UI) puedan reaccionar.
- **Factory Pattern**: Utilizado para crear instancias de componentes del motor, como actores, tableros y misiones.
- **Dependency Injection (DI)**: Facilita la resolución de dependencias dinámicas, permitiendo una configuración flexible y pruebas más sencillas.

#### Aislamiento de la Personalización del Juego
- `BarelyAlive.TurnForge` extiende el motor mediante la definición de comandos, handlers y eventos específicos del juego.
- Las reglas específicas del juego están encapsuladas en esta capa, evitando contaminar el núcleo del motor.

#### Posicionamiento de los Adapters
- `TurnForge.Adapters.Godot` actúa como una capa de traducción entre Godot y el motor.
- Los adapters convierten inputs de la UI en comandos del motor y traducen eventos del motor en datos consumibles por Godot.
- Este diseño asegura que ni el motor ni Godot dependan directamente uno del otro.

#### Comunicación UI ↔ Engine
- La UI en Godot envía comandos al motor a través de los adapters.
- El motor procesa los comandos y publica eventos que son capturados por los adapters y enviados de vuelta a la UI.
- Este flujo asegura una separación clara de responsabilidades y facilita el mantenimiento.

#### Escalabilidad
- **Multiplayer**: El Command Bus puede extenderse para soportar la sincronización de comandos entre clientes y servidor.
- **IA**: Los Command Handlers pueden incluir lógica para ejecutar comandos generados por la IA.
- **Replays**: El Event Bus puede registrar eventos para reproducir partidas.
- **Modding**: La arquitectura basada en comandos y eventos facilita la extensión del motor mediante la adición de nuevos comandos y handlers.
