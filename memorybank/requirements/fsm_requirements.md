## FSM Requirements

Current implementation of the FSM system needs some adjustments to be able to handle with the game complexity

### Definitions
SystemRootNode: Single node that is the root of the tree.
RootNode: Single node that is the root of the tree of the game side
BranchNode: Node that can have multiple children
LeafNode: End Node, has no childre

### Layout
the FSM layout is as follows
<SystemRootNode Name = "SystemRoot">
    <LeafNode name="InitialState"/>
    <LeafNode name="GamePrepared"/>
    <RootNode name="Gameplay">
        // Section of the Game States
    </RootNode>
</SystemRootNode>

### Nodes features
Every Node has a OnStart and OnExit methods that are called when the node is entered or exited. 
Those methods are to be implemented by the user to handle the logic of the node the signature is as follows
```csharp
NodePhaseResult OnStart(GameState state)
NodePhaseResult OnExit(GameState state)
```
The NodePhaseResult is a struct that contains the following properties
```csharp
public struct NodePhaseResult
{
    public PhaseResult PhaseResult { get; set; }
    public Command? CommandToBeLaunched() { get; set; }
    public Decision? DecisionToBApplied() { get; set; }
}

public enum PhaseResult 
{
Pass,
LaunchCommand,
ApplyDecisions,
}
```
Depending on the result of the node, the FSM will execute the corresponding action

#### BrachNode
BrachNode is usually a passtrought node, usually it wont execute any action, and will navigate to the next node.
Has OnStart and OnExit methods that are called when the node is entered or exited. 
Due to usually has no action, it has not a list of validated commands to execute, instead it allows any kind of command
All commands are allowed due to no User command can be sent in this state, due to FSM navigates until a LeafNode is reached.
The only commands that can be executed when we are in this node are the ones requestes by the node itselfs via NodePhaseResult.CommandToBeLaunched

#### LeafNode
Once a LeafNode is reached, the system will stop its navigation and wait for the user to provide a command to execute.
The main loop will execute the command and will call 
```csharp
 var appliersOnCommand = leaf.OnCommandExecuted(command, result, out bool transitionRequested);
```
applicerCommand has the deicions that has to be executed and the transitionRequested flag that indicates FSM must navigate to the next node.
Has OnStart and OnExit methods that are called when the node is entered or exited. 

### Navigation Process (once we are in the game section)
1) Root is the root of the tree, it check if the GameisOver calling to CheckGameOver(GameState state) method. If it returns true the process stops if not
it will navigate to the next node
2) Wait to user to provide a command to execute.
3) The main loop checks if the command is valid in the current state and launch the command if it is valid.
4) Once the command is executed main loop will call the FSM to manage the result
```csharp
 public FsmStepResult HandleCommand(ICommand command, GameState state, CommandResult result)
```
5) Initailly 
```csharp
 var appliersOnCommand = node.OnCommandExecuted(command, result, out bool transitionRequested);
```
is excute and returns if extra decisions have to be applied and if the tree needs to be moved to the next node, performing OnExuit call
If the next node is a BrachNode we will perform OnStart call,
- If is a Pass we will continue to the next node.
- If is a Decision we will execute the decision and will move to the next node.
- If is a Command we will execte the reamining decisions to keep the state consistent and will stop the navigation, proving in rge FsmResult the commabd to be launched.

6) Orchestor will execte the decisions based on the TimingDecsion parameter
```csharp
 if (_orchestrator != null)
{
    _orchestrator.SetState(state); // Sync if legacy changed it
    effects.AddRange(_orchestrator.ExecuteScheduled(null, "OnCommandExecutionEnd"));
    _logger?.Log("[FsmController] Orchestrator OnCommandExecutionEnd executed effects size" + effects.Count);
    state = _orchestrator.CurrentState;
}
```
7) If transitionRequested is true, the FSM will navigate to the next node.
```csharp
if (transitionRequested)
{
    return ExecuteTransition(_currentStateId, nextId.Value, state);
}
```
8) During the transition, and due to OnStart and OnExit methods, is possible that:
- Pass: nothing
- Decision: new decisions are added to the list and must be executed before the transition is completed
- Command: stop an return the command to be executed
9) Continue until the tree is completed and come back to the root node.


