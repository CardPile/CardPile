// using UnitySpy;
using NLog;

namespace CardPile.Watchers.Unity;

internal static class UnitySpyHelpers
{
    /*
    public static T? TryToGetValue<T>(this IManagedObjectInstance instance, string fieldName, string? typeFullName = default) where T : notnull
    {
        #pragma warning disable CS8604 // Possible null reference argument.
        var field = instance.TypeDefinition.GetField(fieldName, typeFullName);
        #pragma warning restore CS8604 // Possible null reference argument.
        if (field == null)
        {
            logger.Warn("Could not find field named {fieldName} of type {fieldType} in type {typeName}", fieldName, typeFullName ?? "<any>", instance.TypeDefinition.FullName);
            return default;
        }

        return instance.GetValue<T>(fieldName, typeFullName);
    }
    */

    /*
    public static IManagedObjectInstance? TryToGetPath(this IManagedObjectInstance instance, params string[] fieldNames)
    {
        IManagedObjectInstance currentInstance = instance;
        foreach (var fieldName in fieldNames)
        {
            var currentField = currentInstance.TryToGetValue<IManagedObjectInstance>(fieldName);
            if (currentField == null)
            {
                return null;
            }
            currentInstance = currentField;
        }
        return currentInstance;
    }
    */

    private static readonly Logger logger = LogManager.GetCurrentClassLogger();
}
