using System;

namespace SurvivalRider.exceptions;


public abstract class ParsingException(string message) : Exception($"Error parsing; {message}");
public abstract class KeyNotFoundException(string id, string message) : Exception($" {message} : Id {id} not found:");
public abstract class ModelException(string message) : Exception($"Model Error: {message} ");

public class ParsingToViewUnitTypeException(string value) : ParsingException($"{value} to ViewUnitType");
public class AreaNotFoundException(Guid id) : KeyNotFoundException(id.ToString(), "Area") { }
public class SurvivorlNameNotFoundException(string name) : KeyNotFoundException(name, "Survival name") { }
public class SceneNotFoundException(string name) : KeyNotFoundException(name, "Scene") { }
public class ResourceNotFoundException(string name) : KeyNotFoundException(name, "Resource") { }
public class AreaTooSmallException(string message) : ModelException(message) { }



