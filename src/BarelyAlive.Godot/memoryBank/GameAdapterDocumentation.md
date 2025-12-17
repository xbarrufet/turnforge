# Game Adapter Documentation
Aquest document descriu com es la conexio de BarelyAlive.Godot ammb el GameEngine TurnForge.Engine
## Estructura carpetes
- BarelyAlive.Godot/src: conte el codi de la aplicacio Godot encarrageda el UI-> nom: BarelyAlive
- BarelyAlive.Godot/TurnForge.GodotAdapter: conte la codi per adaptar el GameEngine a Godot -> nom Adapter
- TurnForge.Engine: Conte el codi font del Game Engine:->nom Engine

### Process de comnicacio
La comunicacio es de BarelyAlive -> Adapter -> Engine i la repsosta Engine -> Adapter -> BarelyAlive
ENVIAMENT
1- BalrelyAlive crea els DTO dels parametres del command que vol cridar, aquests DTO es troben al codi font de Adapter
2- Dins de Adapter hi ha els Mappers que transformen els parametres DTO als parametres esperats pels COmmand
3- GameAdapter monta el command i crida a GameEngine amb el coomand com a parametre pq el posi al command bus
RETORN
4- GameEngine retorna el resultat al GameAdapter
5- GameAdapter pasa el resultat a GameAdaterSignalEmitter que el transforma en clases que poden ser enviades via Godot Signals i l'emet.
7-  GameAdaterSignalEmitter exposa un interface per qie les clases es poguin subscriure als signals
