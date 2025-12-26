Aquí tienes el documento técnico redactado en inglés, diseñado específicamente para ser ingerido por un "Memory Bank" o base de conocimiento de desarrollo.

Está estructurado para ser una referencia rápida y definitiva sobre la arquitectura **TurnForge**.

---

# 📄 ARCHITECTURE_GUIDELINES_STATE_VS_BEHAVIOUR.md

## 🏛️ TurnForge Architecture: Behaviours vs. Attributes (Components)

This document establishes the canonical guidelines for distinguishing between **AttributeComponents** (State) and **Behaviours** (Logic/Identity) within the TurnForge Engine.

### 1. The Golden Rule

To decide where a property belongs, ask: **"Does this change during the game loop?"**

| Feature | **AttributeComponent** | **Behaviour** |
| --- | --- | --- |
| **Concept** | **The State** (How am I now?) | **The Definition/Identity** (What am I?) |
| **Mutability** | **High**. Changes constantly (per turn/action). | **Low/None**. Static configuration or Logic rules. |
| **Persistence** | **Must be Saved**. Serialized in `SaveGame.json`. | **Reconstructed**. Loaded from `Mission.json` or Factory. |
| **Role** | The **Vessel** (Holds the data). | The **Modifier** (Defines the rules). |

---

### 2. Detailed Decision Matrix

#### Case A: Health and Durability

* **Current HP / Wounds:** ➡️ **Component** (`HealthComponent`).
* *Reason:* It changes when damage is taken. Needs to be saved.


* **Max HP / Resilience:** ➡️ **Behaviour** (`BaseStatsBehaviour` or `AgentDefinition`).
* *Reason:* It is intrinsic to the unit type. It doesn't change unless a permanent upgrade is applied.


* **Regeneration:** ➡️ **Behaviour** (`RegenerationBehaviour`).
* *Reason:* It is logic ("Heal 1 at EndOfTurn"). It is not a value that drains.



#### Case B: Movement

* **Action Points (AP) / Movement Left:** ➡️ **Component** (`ActionPointsComponent`).
* *Reason:* It is a resource consumed during the turn.


* **Movement Speed (e.g., 6 inches, 3 zones):** ➡️ **Behaviour** (`BaseStatsBehaviour`).
* *Reason:* It defines the *capability* of the unit.


* **Flight / Phasing:** ➡️ **Behaviour** (`FlyingBehaviour`).
* *Reason:* It is a rule override (ignore terrain costs).



#### Case C: Items and Equipment

* **Ammo Count:** ➡️ **Component** (`AmmunitionComponent`).
* *Reason:* Decreases with every shot.


* **Range & Base Damage:** ➡️ **Behaviour** (`RangedWeaponBehaviour`).
* *Reason:* A "Sniper Rifle" always has Range 30. This is its identity.


* **"Noisy" Trait:** ➡️ **Behaviour** (`NoisyBehaviour`).
* *Reason:* It is a tag that triggers side effects (Spawn Noise Token).



---

### 3. The Implementation Pattern: "The Stat Pipeline"

Do not access "Stats" directly. Use **Strategies** to calculate the final value by combining Components (State), Static Config (Behaviours), and Modifiers (Skills).

#### ❌ Wrong Approach

```csharp
// BAD: Mixing state and definition in one place
public class Unit {
    public int Movement = 6; // Is this current? Max? Base?
}

```

#### ✅ Correct Approach (The Pipeline)

```csharp
public class MovementStrategy {
    public int GetCurrentMoveCapacity(Agent agent) {
        // 1. BASE (From Behaviour/Config)
        // "What is this unit physically capable of?"
        int move = agent.Behaviours.Get<BaseStatsBehaviour>().MoveSpeed;

        // 2. STATE PENALTIES (From Components)
        // "Is the unit injured?"
        var health = agent.GetComponent<HealthComponent>();
        if (health.IsInjured) {
            move -= 2;
        }

        // 3. DYNAMIC MODIFIERS (From Skill Behaviours)
        // "Does it have a temporary buff or passive skill?"
        if (agent.Behaviours.Has("AdrenalineShot")) {
            move += 1;
        }

        return move;
    }
}

```

---

### 4. Domain Specific Examples

#### 🧟 Zombicide Context

* **Skill: "+1 Damage"**: Implemented as a **Behaviour** (Tag). The `CombatStrategy` checks for this tag to add `+1` to the calculation. It is **not** a variable `Damage = Damage + 1` stored on the character.
* **Inventory**: The list of items is a **Component** (`InventoryComponent`). The capabilities of those items (Opening doors, making noise) are **Behaviours** on the Item Entities.

#### 💀 Kill Team Context

* **Operative Datacard (M, APL, GA, DF, SV, W):** These are **Behaviours** (Static Definition).
* **Current Wounds / Injured State:** This is a **Component**.
* **Orders (Engage/Conceal):** This is a **Component** (State changes every Turning Point).
* **Special Rules (Fly, Camo Cloak):** These are **Behaviours**.

### 5. Summary Checklist

1. Is this value needed to restore the game after loading a save file?
* **Yes:** It belongs in a **Component**.
* **No:** It belongs in a **Behaviour** (or Definition).


2. Does this define *what* the object is (e.g., A Rifle)?
* **Yes:** **Behaviour**.


3. Does this define *how* the object is right now (e.g., Empty)?
* **Yes:** **Component**.




## discissions about traits

### 1. proper definition of Trait
>dime los Traits que deberia tener TurnForge ya definidos de base, que se usen en la gran mayoria de juegos, evidentemente el developer podria extenderlos, o crear nuevos


Tienes toda la razón en dudar. Aquí es donde muchos desarrolladores tropiezan.

Si aplicamos la **Regla de Oro** ("Cómo estoy" vs "Qué soy") de forma estricta, tu intuición te está diciendo que `MaxHP` suena a "Definición", pero a veces sientes que debería estar en el componente.

Vamos a resolverlo definitivamente para **TurnForge**.

### La Respuesta Corta

**`MaxHP` NO va en el `HealthComponent`.**
Va en el **`Trait` (Behaviour)**.

El `HealthComponent` solo debe guardar el `CurrentHP`.

---

### La Explicación usando La Regla

#### 1. Analicemos `MaxHP`

Pregúntale a la variable: *"¿Cambias cuando recibo un golpe?"*

* **No.** Si tengo 5/5 de vida y me pegan 1 daño, paso a 4/5. El "5" (Max) no cambió.
* **Conclusión:** El 5 define **QUÉ SOY** (Soy un Tanque, no un Corredor). Por tanto, es Identidad -> **Trait**.

#### 2. Analicemos `CurrentHP`

Pregúntale a la variable: *"¿Cambias cuando recibo un golpe?"*

* **Sí.** Es efímero.
* **Conclusión:** Define **CÓMO ESTOY** (Estoy herido). Por tanto, es Estado -> **Component**.

### ¿Cómo funciona el Spawn (La Creación)?

Cuando el juego arranca y hace *Spawn* de un Zombi:

1. **El Factory lee la Definición (Trait):** "Ah, es un 'Gordo'. Su `BaseStatsTrait` dice que `MaxHP = 2`".
2. **El Factory crea el Componente (State):** "Voy a instanciar un `HealthComponent` nuevo".
3. **La Asignación Inicial:** "Como su máximo es 2, inicializo `CurrentHP = 2`".

El componente nace copiando el valor del Trait, pero **no guarda el Max**, solo su estado actual.

---

### La Implementación Correcta (El Patrón "Stat Provider")

Si pones `MaxHP` en el Componente, creas un problema grave: **Duplicidad de Datos**. Si actualizas el balance del juego y dices "Ahora los Gordos tienen 3 de vida", tendrías que actualizar el JSON *y* todos los savegames.

Si lo separas, el código se ve así:

#### A. Los Datos (Separados)

```csharp
// 1. IDENTIDAD (Trait/Behaviour) - Configuración Estática
public class VitalityTrait : ITrait 
{
    public int BaseMaxHP { get; set; } // Ejemplo: 3
}

// 2. ESTADO (Component) - Lo que se guarda en SaveGame
public class HealthComponent : IComponent 
{
    public int CurrentHP { get; set; } // Ejemplo: 2 (Está herido)
    // ¡Fíjate que aquí NO está MaxHP!
}

```

#### B. La Lógica (Calculada)

Cuando quieras dibujar la barra de vida en la UI, no lees el componente directamente. Usas una **Strategy** o un **Helper**:

```csharp
public class HealthStrategy 
{
    // Calcula el Máximo "Efectivo" en tiempo real
    public int GetMaxHP(Agent agent) 
    {
        // 1. Empieza con la base (Trait)
        int max = agent.Traits.Get<VitalityTrait>().BaseMaxHP;

        // 2. Suma modificadores (Items, Skills, Buffs)
        // Ejemplo: Skill "Duro" (+1 Vida)
        if (agent.Traits.Has("ToughSkill")) 
        {
            max += 1;
        }

        return max;
    }

    public bool IsAlive(Agent agent)
    {
        return agent.GetComponent<HealthComponent>().CurrentHP > 0;
    }
}

```

### ¿Cuándo `MaxHP` SÍ va en el Componente?

Hay una excepción a la regla. Si estás haciendo un juego de rol progresivo (como *Skyrim* o *XCOM*) donde **cada unidad individual evoluciona de forma única y permanente**.

* Si Juan el Soldado sube de nivel y gana +1 HP permanente, y Pedro el Soldado (misma clase) no sube, entonces ese +1 HP ya es parte de la **Historia Individual** de Juan.
* En ese caso, necesitas un `PermanentStatsComponent` que guarde los "Bonus adquiridos".
* Pero el `BaseMaxHP` original sigue viniendo del Trait de su clase.

**Para Zombicide y Kill Team:**
Las unidades son estáticas. Un "Walker" siempre tiene 1 HP. Un "Marine" siempre tiene 12 Wounds (salvo equipo).
**Mantén `MaxHP` en el Trait.**


### 2. Possible initial set of tratis offered by TurForge
Esta es una lista curada de los **Traits (Rasgos)** esenciales que **TurnForge** debería incluir en su librería estándar (`TurnForge.Core.StandardTraits`).

Estos Traits cubren el 90% de las necesidades de cualquier juego de tablero táctico (*Tactical RPG*, *Skirmish*, *Dungeon Crawler*), permitiendo prototipar rápido sin reinventar la rueda.

---

### 1. 🧬 Traits de Identidad y Grupo (Identity)

Definen *qué es* la entidad dentro de la taxonomía del juego.

| Trait | Propiedades Sugeridas | ¿Para qué sirve? |
| --- | --- | --- |
| **`TagTrait`** | `HashSet<string> Tags` | **Fundamental.** Sistema de etiquetas genérico. Ej: "Biological", "Mechanical", "Undead", "Elite". Las Skills buscarán estas etiquetas (ej: "Daño x2 vs Undead"). |
| **`FactionTrait`** | `string FactionId`<br>

<br>`bool IsNeutral` | Define lealtad base. Ej: "Survivors", "Imperium", "Aliens". Vital para la IA (saber a quién atacar). |
| **`UniqueIdentityTrait`** | `string CharacterName`<br>

<br>`string Description` | Para Héroes o Jefes con nombre propio ("Aragorn", "Abominación Tóxica"). Diferencia una instancia específica de la masa genérica. |

### 2. ❤️ Traits de Supervivencia (Vitality & Defense)

Definen cuánto castigo puede aguantar y cómo se defiende.

| Trait | Propiedades Sugeridas | ¿Para qué sirve? |
| --- | --- | --- |
| **`VitalityTrait`** | `int BaseMaxHP`<br>

<br>`bool IsImmortal` | El tope de vida. El componente `Health` se inicializa con este valor. |
| **`ArmorTrait`** | `int DefenseValue`<br>

<br>`int SaveRoll` (ej: 4+) | Define la mitigación de daño pasiva. En Kill Team es el "Save (SV)", en Zombicide no existe en supervivientes (tiran ellos), pero sí en Zombis (resistencia al daño). |
| **`ResilienceTrait`** | `List<DamageType> Immunities`<br>

<br>`List<DamageType> Weaknesses` | Define interacciones elementales. Ej: Inmune a `Fire`, Débil a `Holy`. |

### 3. 🦶 Traits de Espacio y Movimiento (Spatial)

Definen cómo ocupa espacio y cómo se desplaza.

| Trait | Propiedades Sugeridas | ¿Para qué sirve? |
| --- | --- | --- |
| **`BodySizeTrait`** | `int SizeCategory` (S, M, L)<br>

<br>`float Radius/Width` | Define colisiones. Un "Gordo" (Large) podría bloquear un pasillo estrecho donde un "Corredor" (Medium) no. |
| **`LocomotionTrait`** | `int BaseMoveSpeed`<br>

<br>`MoveType Type` (Walk, Fly, Hover) | Define la capacidad base de movimiento y si ignora terreno (Fly). |
| **`BlockerTrait`** | `bool BlocksLineOfSight`<br>

<br>`bool BlocksMovement` | Para escenografía (Muros, Obstáculos) o unidades grandes. Define si se puede ver o pasar a través de él. |

### 4. ⚡ Traits de Acción y Economía (Economy)

Definen qué puede hacer en su turno.

| Trait | Propiedades Sugeridas | ¿Para qué sirve? |
| --- | --- | --- |
| **`ActionResourceTrait`** | `int BaseAP` (Puntos Acción)<br>

<br>`int RecoveryRate` | Define cuántas acciones tiene por turno. Zombicide (3), Kill Team (APL 2 o 3). |
| **`InitiativeTrait`** | `int BaseSpeed`<br>

<br>`int ActivationPriority` | Para determinar el orden de turno. Unos tienen prioridad sobre otros. |

### 5. 🎒 Traits de Objetos e Inventario (Item & Inventory)

Para diferenciar entre un actor y una espada.

| Trait | Propiedades Sugeridas | ¿Para qué sirve? |
| --- | --- | --- |
| **`ItemTrait`** | `float Weight`<br>

<br>`int MaxStack`<br>

<br>`ItemCategory Category` | Marca a la entidad como "recogible". Define si cabe en el inventario. |
| **`EquipmentSlotTrait`** | `EquipmentSlot ValidSlots` (Hand, Body, Head) | Define dónde se puede equipar este ítem. |
| **`InventoryCapacityTrait`** | `int MaxSlots`<br>

<br>`float MaxWeight` | Para Agentes. Define cuánto pueden cargar. |

### 6. ⚔️ Traits de Ofensiva (Offensive)

Para armas o monstruos con ataques naturales.

| Trait | Propiedades Sugeridas | ¿Para qué sirve? |
| --- | --- | --- |
| **`WeaponTrait`** | `int Damage`<br>

<br>`RangeRange Range`<br>

<br>`int AttacksCount` | El trait estándar para cualquier cosa que haga daño. Sirve para armas Melee y Ranged (usando Range 0 para melee). |
| **`AreaEffectTrait`** | `Shape Shape` (Circle, Cone)<br>

<br>`float Radius` | Para granadas, lanzallamas o hechizos. Define el área de impacto. |

---

### Ejemplo de Configuración (Zombicide "Fatty")

Así se vería una definición usando estos Base Traits:

```csharp
var fattyDefinition = new AgentDefinition("zombie_fatty")
{
    Traits = new TraitCollection()
    {
        // 1. Qué es
        new TagTrait("Zombie", "Undead"),
        new FactionTrait("Enemy"),
        
        // 2. Stats
        new VitalityTrait(baseMaxHP: 2), // Cuesta 2 daños matarlo
        
        // 3. Físico
        new BodySizeTrait(SizeCategory.Large), // Ocupa más espacio
        new LocomotionTrait(speed: 1, type: MoveType.Walk), // Lento
        
        // 4. Acción
        new ActionResourceTrait(baseAP: 1), // 1 acción por turno
        
        // 5. Ataque (No lleva arma, es su propio cuerpo)
        new WeaponTrait(damage: 2, range: 0, attacks: 1) // Pega fuerte
    }
};

```

### ¿Por qué estos y no otros?

Estos traits responden a preguntas que el **Engine** (Orchestrator/Strategies) hace constantemente:

1. *MovementStrategy:* "¿Puede volar (`Locomotion`)?"
2. *CombatStrategy:* "¿De qué bando es (`Faction`)? ¿Cuánto daño hace (`Weapon`)?"
3. *InteractionStrategy:* "¿Es un objeto (`Item`)? ¿Cabe en la mochila (`InventoryCapacity`)?"

Cualquier cosa más específica (ej: `VampiricRegenTrait`) debería ser una extensión creada por el desarrollador (`MyGame.Traits`), pero **TurnForge** debe proveer los ladrillos básicos para construir la casa.