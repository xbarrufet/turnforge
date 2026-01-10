namespace TurnForge.Engine.Core.Exceptions;

/// <summary>
/// Exception thrown when a descriptor is invalid for entity creation.
/// This includes missing required traits, uninitialized traits, or invalid configuration.
/// </summary>
public class InvalidDescriptorException : Exception
{
    public InvalidDescriptorException(string message) : base(message)
    {
    }

    public InvalidDescriptorException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
