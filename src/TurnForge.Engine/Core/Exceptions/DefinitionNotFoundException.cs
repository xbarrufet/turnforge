namespace TurnForge.Engine.Core.Exceptions;

public class DefinitionNotFoundException:Exception
{
    public DefinitionNotFoundException(string definitionId,string definitionType) : base($"The definition '{definitionId}' of type {definitionType} was not found.")
    {}
    
    public DefinitionNotFoundException(string message):base(message)
    {}
}