# Backlog de Turnforge
## EPIC-001 :Servei
TurForge te serves que poden ser cridats desde qualsevol Strategy (o Applier?). Per exmple
- VisibiltService (LOS)
- GameStateQueryService
- PathfindingService
- FSMService

Aquest serveix ofereixen metodes per poder obtenir información del GameState recurrent i complicada
*No* permeten la mutación del GameState

## IDEA-001 Effects
Beahaviours que modifiquen l'estat de joc. Tene temporalitat
Effects = IBehaviour + IApplier ???

## FT-001 State confirmation using ACK
Els canvis s'han d'enviar en bloc per asegurar que rebem l'ack adecuat per part del jugador. 

## FT-002 tipus de props del engine
SpawnAgent

## FT-003 tipus de desccripors del engine
PositionedDescriptor, HealthDescriptor

## FT-004:discovery del target de una propietat pel nom
cada propietat del descriptor s'analiyza si te un target

la idea seria

1) Resitro Component (ex PositionComponent --> hash["CurrentPosition"] = SetCurrentPosition

2) descriptor MyDescriptor { string CurrentPosition}

3) GenericAgenFactory.Build(MyDescriptor)
``` pseudocode 
    newAgent = new Agent()
    definition = DefinitionsCatalog[MyDescriptor.definitionId]
    foreach Property in definition
        ' check if the proerty has a decoratot, it has priority over the property name matching
        if it has decorator
           setter = hash[decorator.target]
        else
           setter = hash[Property]
        validate property.value
        setter.Apply(entity, property.value)
    foreach Property in MyDescriptor
        ' check if the proerty has a decoratot, it has priority over the property name matching
        if it has decorator
           setter = hash[decorator.target]
        else
           setter = hash[Property]
        validate property.value
        setter.Apply(entity, property.value)
```