namespace TurnForge.Engine.Registration;

/// <summary>
/// Interfaz para registrar dinámicamente definiciones del juego.
/// </summary>
public interface IDefinitionRegistry<TTypeId, TDefinition>
{
    TDefinition Get(TTypeId typeId);
    bool TryGet(TTypeId typeId, out TDefinition definition);
    void Register(TTypeId id, TDefinition definition);
}


