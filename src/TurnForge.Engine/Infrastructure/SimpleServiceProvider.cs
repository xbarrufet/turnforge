using System;
using System.Collections.Generic;

namespace TurnForge.Engine.Infrastructure;

public sealed class SimpleServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, Func<object>> _factories = new();

    // Registrar instancia singleton
    public void RegisterSingleton<T>(T instance)
        where T : notnull
    {
        _factories[typeof(T)] = () => instance!;
    }

    // Registrar factory (transient o custom)
    public void Register<T>(Func<IServiceProvider, T> factory)
        where T : notnull
    {
        _factories[typeof(T)] = () => factory(this)!;
    }

    public object? GetService(Type serviceType)
    {
        if (_factories.TryGetValue(serviceType, out var factory))
            return factory();

        throw new InvalidOperationException(
            $"Service not registered: {serviceType.Name}");
    }
}