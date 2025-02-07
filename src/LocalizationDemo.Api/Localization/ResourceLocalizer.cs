using System;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace LocalizationDemo.Api.Localization;

/// <summary>
/// Interface for the resource localizer class
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResourceLocalizer<T>
{
    string Localize(string key);
}

/// <summary>
/// Generic resource localizer class to localize resources based on the given key
/// </summary>
/// <typeparam name="T">Any class, registered in the Markers.cs file</typeparam>
public class ResourceLocalizer<T> : IResourceLocalizer<T>
{
    private readonly IStringLocalizer _localizer;
    
    public ResourceLocalizer(IStringLocalizerFactory factory)
    {
        var type = typeof(T);
        var assemblyName = new AssemblyName(type.Assembly.FullName!);
        _localizer = factory.Create(type.Name, assemblyName.Name!);
    }

    // Localize the given key
    public string Localize(string key)
    {
        return _localizer[key];
    }
}
