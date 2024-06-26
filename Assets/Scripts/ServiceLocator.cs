using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class ServiceLocator : MonoBehaviour
{
    public static ServiceLocator Instance;
    private readonly Dictionary<Type, object> _services = new();

    private void Awake()
    {
        Instance = this;
    }
    
    public void RegisterService<T>(T service)
    {
        var type = typeof(T);
        if (!_services.TryAdd(type, service))
        {
            Debug.LogWarning($"Service of type {type} is already registered.");
        }
    }

    public T GetService<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }

        Debug.LogError($"No service of type {type} registered.");
        return default;
    }
}