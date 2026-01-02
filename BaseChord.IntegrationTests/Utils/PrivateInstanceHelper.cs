using System.Reflection;

namespace BaseChord.IntegrationTests.Utils;

public static class PrivateInstanceHelper
{
    public static T CreateInstance<T>(params object[] args)
    {
        var type = typeof(T);
        var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, args.Select(a => a.GetType()).ToArray(), null);

        if (constructor == null)
        {
            throw new InvalidOperationException($"No constructor found for type {type.Name} with the specified arguments.");
        }

        return (T)constructor.Invoke(args);
    }

    public static T SetProperty<T>(this T instance, string propertyName, object value)
    {
        var type = typeof(T);
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (property == null)
        {
            throw new InvalidOperationException($"Property {propertyName} not found on type {type.Name}.");
        }

        property.SetValue(instance, value);

        return instance;
    }
}
