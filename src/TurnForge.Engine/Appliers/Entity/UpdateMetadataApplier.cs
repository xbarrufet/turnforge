using TurnForge.Engine.Appliers.Entity.Interfaces;
using TurnForge.Engine.Appliers.Entity;
using TurnForge.Engine.Core.Interfaces;
using TurnForge.Engine.Entities;
using TurnForge.Engine.Appliers.Entity.Results.Interfaces;

namespace TurnForge.Engine.Appliers.Entity
{
    public class UpdateMetadataApplier : IFsmApplier
    {
        private readonly string _key;
        private readonly object _value;

        public UpdateMetadataApplier(string key, object value)
        {
            _key = key;
            _value = value;
        }

        public ApplierResponse Apply(GameState state)
        {
            var newState = state.WithMetadata(_key, _value);
            return new ApplierResponse(newState, Array.Empty<IGameEvent>());
        }
    }
}
