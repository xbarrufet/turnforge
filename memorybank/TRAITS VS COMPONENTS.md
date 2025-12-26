Aquí tienes el documento técnico redactado en inglés, diseñado específicamente para ser ingerido por un "Memory Bank" o base de conocimiento de desarrollo.

Está estructurado para ser una referencia rápida y definitiva sobre la arquitectura **TurnForge**.

---

# 📄 ARCHITECTURE_GUIDELINES_STATE_VS_TRAIT.md

## 🏛️ TurnForge Architecture: Traits vs. Attributes (Components)

This document establishes the canonical guidelines for distinguishing between **AttributeComponents** (State) and **Traits** (Logic/Identity) within the TurnForge Engine.

### 1. The Golden Rule

To decide where a property belongs, ask: **"Does this change during the game loop?"**

| Feature | **AttributeComponent** | **Trait** |
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


* **Max HP / Resilience:** ➡️ **Trait** (`BaseStatsTrait` or `AgentDefinition`).
* *Reason:* It is intrinsic to the unit type. It doesn't change unless a permanent upgrade is applied.


* **Regeneration:** ➡️ **Trait** (`RegenerationTrait`).
* *Reason:* It is logic ("Heal 1 at EndOfTurn"). It is not a value that drains.



#### Case B: Movement

* **Action Points (AP) / Movement Left:** ➡️ **Component** (`ActionPointsComponent`).
* *Reason:* It is a resource consumed during the turn.


* **Movement Speed (e.g., 6 inches, 3 zones):** ➡️ **Trait** (`BaseStatsTrait`).
* *Reason:* It defines the *capability* of the unit.


* **Flight / Phasing:** ➡️ **Trait** (`FlyingTrait`).
* *Reason:* It is a rule override (ignore terrain costs).



#### Case C: Items and Equipment

* **Ammo Count:** ➡️ **Component** (`AmmunitionComponent`).
* *Reason:* Decreases with every shot.


* **Range & Base Damage:** ➡️ **Trait** (`RangedWeaponTrait`).
* *Reason:* A "Sniper Rifle" always has Range 30. This is its identity.


* **"Noisy" Trait:** ➡️ **Trait** (`NoisyTrait`).
* *Reason:* It is a tag that triggers side effects (Spawn Noise Token).



---

### 3. The Implementation Pattern: "The Stat Pipeline"

Do not access "Stats" directly. Use **Strategies** to calculate the final value by combining Components (State), Static Config (Traits), and Modifiers (Skills).

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
        // 1. BASE (From Trait/Config)
        // "What is this unit physically capable of?"
        int move = agent.Traits.Get<BaseStatsTrait>().MoveSpeed;

        // 2. STATE PENALTIES (From Components)
        // "Is the unit injured?"
        var health = agent.GetComponent<HealthComponent>();
        if (health.IsInjured) {
            move -= 2;
        }

        // 3. DYNAMIC MODIFIERS (From Skill Traits)
        // "Does it have a temporary buff or passive skill?"
        if (agent.Traits.Has("AdrenalineShot")) {
            move += 1;
        }

        return move;
    }
}

```

---

### 4. Domain Specific Examples

#### 🧟 Zombicide Context

* **Skill: "+1 Damage"**: Implemented as a **Trait** (Tag). The `CombatStrategy` checks for this tag to add `+1` to the calculation. It is **not** a variable `Damage = Damage + 1` stored on the character.
* **Inventory**: The list of items is a **Component** (`InventoryComponent`). The capabilities of those items (Opening doors, making noise) are **Traits** on the Item Entities.

#### 💀 Kill Team Context

* **Operative Datacard (M, APL, GA, DF, SV, W):** These are **Traits** (Static Definition).
* **Current Wounds / Injured State:** This is a **Component**.
* **Orders (Engage/Conceal):** This is a **Component** (State changes every Turning Point).
* **Special Rules (Fly, Camo Cloak):** These are **Traits**.

### 5. Summary Checklist

1. Is this value needed to restore the game after loading a save file?
* **Yes:** It belongs in a **Component**.
* **No:** It belongs in a **Trait** (or Definition).


2. Does this define *what* the object is (e.g., A Rifle)?
* **Yes:** **Trait**.


3. Does this define *how* the object is right now (e.g., Empty)?
* **Yes:** **Component**.