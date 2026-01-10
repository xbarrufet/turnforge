using TurnForge.Engine.Core.Exceptions;
using TurnForge.Engine.Entities.Board.Topology.Interfaces;
using TurnForge.Engine.Entities.Board.ValueObjects;
using TurnForge.Engine.Entities.Definitions.BasicDefinitions;
using TurnForge.Engine.Entities.Descriptors;
using TurnForge.Engine.Entities.TraitsComponents.Interfaces;

namespace TurnForge.Engine.Entities.Builders;

public class ZoneDescriptorBuilder
{
    private string _name;
    private ZoneId _zoneId;
    private IZoneTopology _zoneTopology = IZoneTopology.Empty;
    private string definitionId = BasicZoneDefinition.DefinitionId;
    private List<ITrait> definitionTraitValues = [];

    public ZoneDescriptorBuilder(ZoneId zoneId)
    {
        _zoneId= zoneId;
        _name = _zoneId.ToString();
    }
    
    public ZoneDescriptorBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public ZoneDescriptorBuilder WithZoneTopology(IZoneTopology zoneTopology)
    {
        _zoneTopology = zoneTopology;
        return this;
    }
    
    public ZoneDescriptorBuilder WithDefinitionId(string definitionId)
    {
        this.definitionId = definitionId;
        return this;
    }

    public ZoneDescriptorBuilder AddTrait(ITrait trait)
    {
        this.definitionTraitValues.Add(trait);
        return this;
    }
    
    public ZoneDescriptor Build()
    {
        _validate();
        return new ZoneDescriptor(
            _name,
            _zoneId,
            _zoneTopology,
            definitionId,
            definitionTraitValues);
    }
    
    private void _validate()
    {
        if(_zoneTopology.Equals(IZoneTopology.Empty))
        {
            throw new InvalidDescriptorException("ZoneTopology must be set before building ZoneDescriptor");
        }
    }   
}