## Implementation of the Action System

We will introduce in the system Action Commands. Action commands are commands done usuarlly by teh players, not by the syste like (spawn).
TurnForge is a turn-based game, so actions are done per turn and they have a cost in terms of actions points. This cost musy be customizable by the game developer

I want to define the requirements using a MOVE command that has to be developed.


## ActionPointsComponent
there is a specific component to manae the ActionPoitns ActionPointsComponent that has initially two fields:
- CurrentActionPoints: int
- MaxActionPoints: int

### MOVE Command
Move command allows a Agent to move from 1 posiion to another position in the board

MoveCommand will have the following properties:
- AgentId: Id of the agent that is moving
- Position To

MoveCommand should implement the IActionCommand interface that extends from ICommand interface with the following properties:
- AgentId: Id of the agent that performs the action
- HasCost: Boolean that indicates if the action has a cost (or cover for instance free movement special ability )


The movement will be validated by the MoveStrategy and it will return the PositionComponent to be udpated as a decision
We want to follow the same pattern as the spawn command, so we will have a MoveDecision and a PositionComponent especific applier

MovemtStategy:IActionStratey 
ProcessAction(AgentDescriptor, MoveCommandm GaneState, GameBoard)-> ActionStrategyResult

ActionStrategyResult has:
- Llista de updates de component amb e Id de la entity a modificar, pot ser mes de 1 (moure -> enemic es mogui tambe)
ActionStrategyResult =>
    List UpdateComponentDecicions
    ExecuteAction: boolean //indicates if the action should be executed. This is more for the UI

UpdateComponentDecicions =>
    EntityId
    List<ComponentType> ComponentTypesToUpdate (it includes the ActionPointCOmponent update)
The updates will be done despite the ExecuteAction property, it is more for the UI to know if the action should be executed or not. The strategy is responsible for deciding what has to be udpated if it fails


the action points are calculated in the strategy as he has all the information needed to calculate the cost, it can't be calculated in configuration as it depends on the action and the context (ex: in Zombicide moving from a tile with zombies has a difierent cost than normal movement)
e moving agent, MoveCommand)

For the appploiers we can start with a generic one, later we will see if we need to create specific ones for each component type