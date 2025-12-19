using System.Collections.Generic;
using TurnForge.Engine.Registration;

namespace TurnForge.Engine.Tests.Helpers
{
    public class TestDefinitionRegistry<TTypeId, TDefinition> : IDefinitionRegistry<TTypeId, TDefinition>
        where TTypeId : notnull
    {
        private readonly Dictionary<TTypeId, TDefinition> _definitions = new Dictionary<TTypeId, TDefinition>();

        public TDefinition Get(TTypeId typeId)
        {
            return _definitions[typeId];
        }

        public bool TryGet(TTypeId typeId, out TDefinition definition)
        {
            return _definitions.TryGetValue(typeId, out definition);
        }

        public void Register(TTypeId id, TDefinition definition)
        {
            _definitions[id] = definition;
        }
    }
}
