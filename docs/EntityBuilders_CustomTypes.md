# ✅ Customizing Agents and Props via Definitions and Descriptors

## 🎯 Nueva Política

No se crearán nuevas clases GameEntity personalizadas por el usuario (no habrá `class Scout : Agent`).

La forma de **customizar** un Agent o Prop será exclusivamente mediante:
- **Definitions**: añadiendo traits de configuración.
- **Descriptors**: añadiendo/inyectando traits y components extra.

En otras palabras:
```
CustomAgent = Agent (base) + CustomDefinition + CustomDescriptor
CustomProp  = Prop  (base) + CustomDefinition + CustomDescriptor
```

---

## 🧩 Cómo Funciona

- El builder siempre crea la **entidad base** (`Agent`/`Prop`).
- Los **traits** del Definition configuran capacidades (p. ej., velocidad, salud, acción).
- El **TraitInitializationService** crea e inicializa componentes a partir de los traits.
- El **Descriptor** puede añadir componentes extra y overrides de traits.

---

## 🚀 Ejemplo: CustomAgent sin subclase

### 1) Definición personalizada
```csharp
public sealed class FastAgentDefinition : AgentDefinition
{
    public FastAgentDefinition(string definitionId) : base(definitionId)
    {
        Traits = new ITrait[]
        {
            new MovableTrait { MaxSpeed = 10 },
            new VitalityTrait { MaxHealth = 120 },
            new ActionPoolTrait { MaxActions = 2 }
        };
    }
}
```

### 2) Descriptor personalizado
```csharp
public sealed class FastAgentDescriptor : AgentDescriptor
{
    public FastAgentDescriptor(string definitionId, string teamId, string playerId)
        : base(definitionId, teamId, playerId)
    {
        // Añadir componentes extra
        ExtraComponents = new List<IGameEntityComponent>
        {
            new ConnectionComponent()
        };
        
        // Overrides de traits
        DefinitionTraitValues = new List<ITrait>
        {
            new MovableTrait { MaxSpeed = 12 } // override
        };
    }
}
```

### 3) Crear el agente
```csharp
var registry = new InMemoryGameCatalog();
registry.RegisterDefinition(new FastAgentDefinition("fast-agent"));

var builder = new AgentBuilder(traitService);
var def = registry.GetDefinition<AgentDefinition>("fast-agent");
var desc = new FastAgentDescriptor("fast-agent", teamId: "blue", playerId: "player_1");

Agent agent = builder.Build(desc, def); // Siempre Agent base + custom traits/components
```

---

## ✅ Beneficios

- Simplicidad: no hay jerarquías complejas de tipos.
- Type-safety: traits y componentes siguen siendo tipos fuertes.
- Consistencia: todas las personalizaciones pasan por definitions y descriptors.
- Rendimiento: construcción siempre usa el fast path del base + inicialización de traits.

---

## ❌ Qué ya no se admite

- No se crean subclases como `class Scout : Agent`.
- No hay `BuildAgent<TAgent>` ni `BuildProp<TProp>`.
- No usar `Activator.CreateInstance` para tipos personalizados.

---

## 🛠️ Migración (si venías de subclases)

- Mueve la lógica específica a **traits** (config) y **components** (estado/behavior).
- Usa el **Descriptor** para inyectar componentes extra y overrides.
- Si necesitas APIs específicas, expón **facades de capability** por componente en vez de subclases.

---

## 📌 Resumen

- Se mantiene `Agent`/`Prop` como clases base únicas.
- Todo el comportamiento se modela con **traits + components**.
- Builders y Factory ya están adaptados para este flujo.

**Personaliza por definición/descriptor, no por herencia.** ✅
