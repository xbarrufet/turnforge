# 🔄 Guía de Actualización: Agent ahora requiere TeamId y PlayerId

## ✅ Cambio Realizado

Se han añadido dos propiedades al constructor de `Agent`:
- `TeamId` (string) - Identificador del equipo
- `PlayerId` (string) - Identificador del jugador/controlador

---

## 📋 Constructor Anterior

```csharp
public Agent(
    EntityId id,
    string definitionId,
    string name,
    string category)
```

## 📋 Nuevo Constructor

```csharp
public Agent(
    EntityId id,
    string definitionId,
    string name,
    string category,
    string teamId,      // ✅ NUEVO
    string playerId)    // ✅ NUEVO
{
    TeamId = teamId;
    PlayerId = playerId;
    // ...
}
```

---

## 🔧 Cómo Actualizar tus Subclases

Si tienes subclases de Agent (Scout, Soldier, Hero, etc.), necesitas actualizar el constructor.

### ANTES

```csharp
public class Scout : Agent
{
    public Scout(
        EntityId id,
        string definitionId,
        string name,
        string category)
        : base(id, definitionId, name, category)  // ❌ 4 parámetros
    {
    }
}
```

### DESPUÉS

```csharp
public class Scout : Agent
{
    public Scout(
        EntityId id,
        string definitionId,
        string name,
        string category,
        string teamId,      // ✅ NUEVO
        string playerId)    // ✅ NUEVO
        : base(id, definitionId, name, category, teamId, playerId)  // ✅ 6 parámetros
    {
    }
}
```

---

## 📝 Ejemplo Completo

```csharp
namespace MyGame.Entities;

public class Warrior : Agent
{
    public Warrior(
        EntityId id,
        string definitionId,
        string name,
        string category,
        string teamId,
        string playerId)
        : base(id, definitionId, name, category, teamId, playerId)
    {
        // Inicialización específica del Warrior
    }
}

public class Mage : Agent
{
    public Mage(
        EntityId id,
        string definitionId,
        string name,
        string category,
        string teamId,
        string playerId)
        : base(id, definitionId, name, category, teamId, playerId)
    {
        // Inicialización específica del Mage
        ManaPoints = 100;
    }
    
    public int ManaPoints { get; set; }
}
```

---

## ✨ Acceso a TeamId y PlayerId

Una vez actualizado, puedes acceder a estas propiedades en tus entidades:

```csharp
var agent = factory.BuildAgent<Scout>(descriptor);

// ✅ Ahora puedes acceder a:
Console.WriteLine($"Team: {agent.TeamId}");
Console.WriteLine($"Controlled by: {agent.PlayerId}");

// ✅ Útil para lógica de juego
if (agent.TeamId == "enemies")
{
    // Enemigos
}

if (agent.PlayerId == "ai_controller")
{
    // Controlado por IA
}
```

---

## ❓ ¿Se Afecta el Builder?

**NO.** El builder (AgentBuilder) ya está actualizado.

```csharp
// ✅ Sigue funcionando igual
var agent = factory.BuildAgent<Scout>(scoutDescriptor);

// El builder automáticamente pasa TeamId y PlayerId
// desde el descriptor al constructor
```

---

## ❓ ¿Se Afecta Prop?

**NO.** Prop no cambia. Solo Agent.

```csharp
// ✅ Sigue siendo igual que antes
var prop = factory.BuildProp<Chest>(chestDescriptor);
```

---

## 🎯 Checklist de Actualización

Si tienes subclases de Agent:

- [ ] Actualiza el constructor para incluir `teamId` y `playerId`
- [ ] Pasa los parámetros al `base()`
- [ ] ✅ **¡Listo!**

```csharp
public MyAgent(EntityId id, string def, string name, string cat, 
               string teamId, string playerId)
    : base(id, def, name, cat, teamId, playerId) ✅
```

---

## ⚠️ Si No Actualizas

Si olvidas actualizar una subclase, obtendrás este error en runtime:

```
InvalidOperationException: Failed to create instance of MyAgent.
Ensure it has a constructor: (EntityId, string, string, string, string, string)
```

**Solución**: Añade los dos parámetros al constructor.

---

## 💡 ¿Por Qué es Importante?

Ahora los Agents saben:
- ✅ A qué equipo pertenecen
- ✅ Quién los controla (jugador, IA, NPC)

Útil para:
- Lógica de equipo/facción
- Distinguir jugadores vs IA
- Sistemas de alineación

---

## 📖 Ejemplo Práctico

```csharp
// Definición
var warriorDef = new AgentDefinition { /* ... */ };

// Creación
var warrior = factory.BuildAgent<Warrior>(
    new AgentDescriptor(
        definitionId: "warrior",
        teamId: "red_team",      // ← Pasa aquí
        controllerId: "player_1"  // ← Pasa aquí
    )
);

// ✅ Ahora puedes usar:
bool isEnemy = warrior.TeamId == "blue_team";
bool isAI = warrior.PlayerId.StartsWith("ai_");

// ✅ Información siempre disponible
foreach (var ally in team)
{
    if (ally.TeamId == warrior.TeamId)
    {
        // Mismo equipo
    }
}
```

---

## ✅ Resumen

| Aspecto | Antes | Ahora |
|---------|-------|-------|
| Constructor Agent | 4 parámetros | 6 parámetros |
| TeamId | ❌ No | ✅ Sí |
| PlayerId | ❌ No | ✅ Sí |
| Builder | Sin cambios | Actualizado ✅ |
| Subclases | Requieren update | Requieren update |

**Cambio simple, beneficio grande.** 🚀

