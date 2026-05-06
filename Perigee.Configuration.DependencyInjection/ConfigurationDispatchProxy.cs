using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Perigee.Configuration.DependencyInjection;

/// <summary>
/// A dynamic proxy that evaluates interface property getters directly against an IConfiguration.
/// </summary>
public class ConfigurationDispatchProxy : DispatchProxy
{
    private IConfiguration _configuration = null!;

    /// <summary>
    /// Creates a proxy for the specified interface, backed by the provided configuration.
    /// </summary>
    public static T Create<T>(IConfiguration configuration) where T : class
    {
        var proxy = Create<T, ConfigurationDispatchProxy>();
        var dispatchProxy = (ConfigurationDispatchProxy)(object)proxy;
        dispatchProxy._configuration = configuration;
        return proxy;
    }

    /// <inheritdoc />
    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod == null || !targetMethod.Name.StartsWith("get_"))
        {
            throw new NotSupportedException("Configuration proxies only support property getters.");
        }

        var propertyName = targetMethod.Name.Substring(4);
        var returnType = targetMethod.ReturnType;

        // If the return type is an interface, return a nested proxy scoped to that section.
        if (returnType.IsInterface)
        {
            var section = _configuration.GetSection(propertyName);
            var createMethod = typeof(ConfigurationDispatchProxy).GetMethod(nameof(Create))!
                .MakeGenericMethod(returnType);
            return createMethod.Invoke(null, new object[] { section });
        }

        // Fetch the raw string value from IConfiguration
        var valueStr = _configuration[propertyName];
        
        if (valueStr == null)
        {
            return returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
        }

        if (returnType == typeof(string))
        {
            return valueStr;
        }

        try
        {
            var converter = TypeDescriptor.GetConverter(returnType);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromString(valueStr);
            }
            
            return Convert.ChangeType(valueStr, returnType);
        }
        catch
        {
            // If conversion fails, return default for the type instead of crashing
            return returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
        }
    }
}
