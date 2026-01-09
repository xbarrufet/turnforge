using TurnForge.Engine.Components.Interfaces;
using TurnForge.Engine.ValueObjects;

namespace TurnForge.Engine.Components;

public class ConnectionComponent:IConnectionComponent
{
    
    public ConnectionPosition Position { get; private set; }
    public bool IsConnecctionOpen { get; private set; }

    public ConnectionComponent(ConnectionPosition position, bool isOpen = false)
    {
        Position = position;
        IsConnecctionOpen = isOpen;
    }

    public void OpenConnection()
    {
        IsConnecctionOpen = true;
    }

    public void CloseConnection()
    {
        IsConnecctionOpen = false;
    }

    public void swithchConnectionState()
    {
        IsConnecctionOpen = !IsConnecctionOpen;
    }
}