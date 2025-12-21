# Backlog de Turnforge
## Servei
TurForge te serves que poden ser cridats desde qualsevol Strategy (o Applier?). Per exmple
- VisibiltService (LOS)
- GameStateQueryService
- PathfindingService
- FSMService

Aquest serveix ofereixen metodes per poder obtenir información del GameState recurrent i complicada
*No* permeten la mutación del GameState

## Effects
Beahaviours que modifiquen l'estat de joc. Tene temporalitat
Effects = IBehaviour + IApplier ???

## State confirmation using ACK
Els canvis s'han d'enviar en bloc per asegurar que rebem l'ack adecuat per part del jugador. 


