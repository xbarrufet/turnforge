# ---

**TF\_ARCH\_CBS\_v1.md \- Estratègia Component-Behaviour-Strategy**

## **1\. Introducció**

Aquest document detalla l'evolució de l'arquitectura de **TurnForge** cap al model **CBS (Component-Behaviour-Strategy)**. Aquest sistema està dissenyat per gestionar la complexitat dels jocs de taula estil *Skirmish*, on les regles són altament dependents del context i de les metadades de les entitats.

## ---

**2\. Definició de Pilars**

### **2.1 Component (Domini i Estat)**

Els components són contenidors de dades agrupats per domini lògic.

* **Responsabilitat**: Gestionar l'estat intern (dades) i respondre preguntes de domini (lògica de consulta).  
* **Accés**: Cada Agent posseeix una col·lecció de components (Salut, Moviment, Combat, etc.).  
* **Modificació**: L'estat d'un component **només** es pot modificar mitjançant un IComponentApplier.

### **2.2 Behaviour (Metadades i Tags)**

Els behaviours deixen de ser lògica executable per convertir-se en **Tags amb Atributs**.

* **BehaviourComponent**: Component universal que conté la llista de tags de l'agent.  
* **Funció**: Serveixen de modificadors per a les Strategies. Un tag com Fly { Speed: 6 } no mou l'agent, però indica a la Strategy que pot ignorar obstacles.

### **2.3 Strategy (Lògica de Regles)**

Les Strategies són els cervells del motor que calculen les conseqüències de les accions.

* **Input**: Reben els components implicats i consulten el BehaviourComponent per afinar el càlcul.  
* **Output**: Una llista de **Appliers**.

## ---

**3\. El Cicle de l'Applier**

Per garantir l'atocimitat i la traçabilitat, el motor segueix aquest flux d'execució:

1. **Càlcul**: El CommandHandler invoca la Strategy pertinent.  
2. **Generació**: La Strategy retorna una col·lecció de IComponentApplier (ex: DamageApplier, StatusEffectApplier).  
3. **Aplicació**: El CommandHandler executa els Appliers sobre els components de l'entitat, actualitzant el GameState.  
4. **Notificació**: Cada Applier aplicat dispara un esdeveniment a l' IEffectSink per a la representació visual a Godot 4\.

## ---

**4\. Mapeig de Regles Skirmish**

| Domini | Component | Behaviours (Tags) | Strategy / Appliers |
| :---- | :---- | :---- | :---- |
| **Moviment** | MovementComponent | Fly, Climb, Stunned | PathfindingStrategy \-\> PositionApplier |
| **Combat** | CombatComponent | Poisonous, Precise | AttackStrategy \-\> DamageApplier \+ TagApplier |
| **Defensa** | DefenseComponent | Covered, Shielded | DefenseStrategy \-\> MitigationApplier |

## ---

**5\. Implementació Tècnica (Pseudocodi)**

C\#

// L'Agent com a contenidor de Components  
public class Agent : Actor {  
    public BehaviourComponent Behaviours { get; } // Conté els Tags  
    public HealthComponent Health { get; }        // Conté la Vida  
}

// L'Applier com a unitat de canvi  
public interface IComponentApplier {  
    void Apply(Agent agent);  
}

---

Vols que comencem a definir la interfície IComponentApplier i com el MoveCommandHandler hauria de gestionar la llista d'appliers que retorni la MovementStrategy?