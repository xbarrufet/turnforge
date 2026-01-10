using TurnForge.Engine.Core.Exceptions;
using TurnForge.Engine.Entities.Board;
using TurnForge.Engine.Entities.Board.Interfaces;
using TurnForge.Engine.Entities.Board.Topology.Discrete;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.Definitions.BasicDefinitions;
using TurnForge.Engine.Entities.Descriptors;

namespace TurnForge.Engine.Entities.Builders;

public class ConnectionDescriptorBuilder
{
    private string _name = "Unnamed Connection";
    private ZoneId _from= ZoneId.Empty;
    private ZoneId _to= ZoneId.Empty;
    private IZoneConnectionPosition _connectionPosition = new EmptyZoneConnectionPosition();
    private string definitionId = BasicConnectionDefinition.DefinitionId;

    public ConnectionDescriptorBuilder()
    {
        
    }
    
    public ConnectionDescriptorBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public ConnectionDescriptorBuilder From(ZoneId from)
    {
        _from = from;
        return this;
    }
    public ConnectionDescriptorBuilder To(ZoneId to)
    {
        _to = to;
        return this;
    }
    public ConnectionDescriptorBuilder WithDiscreteConnectionPoint(Action<DiscreteZoneConnectionPositionBuilder> configure)
    {
        var builder = new DiscreteZoneConnectionPositionBuilder();
        configure(builder);
        _connectionPosition = builder.Build();
        return this;
    }
    public ConnectionDescriptorBuilder WithDefinitionId(string definitionId)
    {
        this.definitionId = definitionId;
        return this;
    }
    public ConnectionDescriptor Build()
    {
        _validate();
        return new ConnectionDescriptor(
            _name,
            _from,
            _to,
            _connectionPosition,
            definitionId);
    }

    private void _validate()
    {
        //zones can't be empty
        if(_from.Equals(ZoneId.Empty))
            throw new InvalidDescriptorException("Connection 'from' ZoneId cannot be empty.");
        if(_to.Equals(ZoneId.Empty))
            throw new InvalidDescriptorException("Connection 'to' ZoneId cannot be empty.");
        // connection position must be set and not empty
        if(_connectionPosition.Equals(IZoneConnectionPosition.Empty))
            throw new InvalidDescriptorException("ConnectionPosition must be set before building ConnectionDescriptor");
        // numnber of connections > 0
        if (_connectionPosition.NumberOfConnections <= 0)
            throw new InvalidDescriptorException(
                "ConnectionPosition must have at least one connection point before building Connection");
    }

}