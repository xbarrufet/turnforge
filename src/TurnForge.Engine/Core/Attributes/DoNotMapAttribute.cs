using System;

namespace TurnForge.Engine.Core.Attributes;

/// <summary>
/// Prevents a property from being automatically mapped by the PropertyAutoMapper.
/// Use this for public properties that should not be set externally via Descriptors/Definitions.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class DoNotMapAttribute : Attribute
{
}
