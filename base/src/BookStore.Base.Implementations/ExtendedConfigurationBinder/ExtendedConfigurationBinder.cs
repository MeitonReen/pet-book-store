using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BookStore.Base.Implementations.ClassDynamicWrapper;
using Microsoft.Extensions.Configuration;

namespace BookStore.Base.Implementations.ExtendedConfigurationBinder;

public static class ExtendedConfigurationBinder
{
    private const BindingFlags DeclaredOnlyLookup = BindingFlags.Public | BindingFlags.NonPublic |
                                                    BindingFlags.Instance | BindingFlags.Static |
                                                    BindingFlags.DeclaredOnly;

    private const string TrimmingWarningMessage =
        "In case the type is non-primitive, the trimmer cannot statically analyze the object's type so its members may be trimmed.";

    private const string InstanceGetTypeTrimmingWarningMessage =
        "Cannot statically analyze the type of instance so its members may be trimmed";

    private const string PropertyTrimmingWarningMessage =
        "Cannot statically analyze property.PropertyType so its members may be trimmed.";

    /// <summary>
    /// Attempts to bind the configuration instance to a new instance of type T.
    /// If this configuration section has a value, that will be used.
    /// Otherwise binding by matching property names against configuration keys recursively.
    /// </summary>
    /// <typeparam name="T">The type of the new instance to bind.</typeparam>
    /// <param name="configuration">The configuration instance to bind.</param>
    /// <param name="configureOptions">Configures the binder options.</param>
    /// <returns>The new instance of T if successful, default(T) otherwise.</returns>
    [RequiresUnreferencedCode(TrimmingWarningMessage)]
    public static T Get<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this IConfiguration configuration, Action<ExtendedBinderOptions> configureOptions)
        where T : class
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        var result = Activator.CreateInstance<T>();
        object options = result;

        var binderOptions = new ExtendedBinderOptions();
        configureOptions?.Invoke(binderOptions);

        if (binderOptions.IncludeRootConfigClassAsConfigProperty)
        {
            options = WrapperObjectToPropertyWrapperStatic.Wrap(result);
        }

        options = BindInstance(typeof(T), options, configuration, binderOptions, out _);

        if (options == default)
        {
            return default(T);
        }

        return result;
    }

    public static void Bind(this IConfiguration configuration, object instance,
        ExtendedBinderOptions configureOptions)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (instance != null)
        {
            BindInstance(instance.GetType(), instance, configuration, configureOptions, out _);
        }
    }

    /// <summary>
    /// Attempts to bind the given object instance to configuration values by matching property names against configuration keys recursively.
    /// </summary>
    /// <param name="configuration">The configuration instance to bind.</param>
    /// <param name="instance">The object to bind.</param>
    /// <param name="configureOptions">Configures the binder options.</param>
    [RequiresUnreferencedCode(InstanceGetTypeTrimmingWarningMessage)]
    public static void Bind(this IConfiguration configuration, object instance,
        Action<ExtendedBinderOptions> configureOptions)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        if (instance != null)
        {
            var options = new ExtendedBinderOptions();
            configureOptions?.Invoke(options);
            BindInstance(instance.GetType(), instance, configuration, options, out _);
        }
    }

    [RequiresUnreferencedCode(PropertyTrimmingWarningMessage)]
    private static void BindNonScalar(this IConfiguration configuration, object instance,
        ExtendedBinderOptions options)
    {
        if (instance != null)
        {
            List<PropertyInfo> modelProperties = GetAllProperties(instance.GetType());

            if (options.ErrorOnUnknownConfiguration)
            {
                HashSet<string> propertyNames = new(modelProperties.Select(mp => mp.Name),
                    StringComparer.OrdinalIgnoreCase);

                IEnumerable<IConfigurationSection> configurationSections = configuration.GetChildren();
                List<string> missingPropertyNames = configurationSections
                    .Where(cs => !propertyNames.Contains(cs.Key))
                    .Select(mp => $"'{mp.Key}'")
                    .ToList();

                if (missingPropertyNames.Count > 0)
                {
                    throw new InvalidOperationException("Error_MissingConfig");
                }
            }

            foreach (PropertyInfo property in modelProperties)
            {
                BindProperty(property, instance, configuration, options);
            }
        }
    }

    [RequiresUnreferencedCode(PropertyTrimmingWarningMessage)]
    private static void BindProperty(PropertyInfo property, object instance, IConfiguration config,
        ExtendedBinderOptions options)
    {
        // We don't support set only, non public, or indexer properties
        if (property.GetMethod == null ||
            (!options.BindNonPublicProperties && !property.GetMethod.IsPublic) ||
            property.GetMethod.GetParameters().Length > 0)
        {
            return;
        }

        object propertyValue = property.GetValue(instance);
        bool hasSetter = property.SetMethod != null && (property.SetMethod.IsPublic || options.BindNonPublicProperties);

        if (propertyValue == null && !hasSetter)
        {
            // Property doesn't have a value and we cannot set it so there is no
            // point in going further down the graph
            return;
        }

        propertyValue = GetPropertyValue(property, instance, config, options);

        if (propertyValue != null && hasSetter)
        {
            property.SetValue(instance, propertyValue);
        }
    }

    [RequiresUnreferencedCode(
        "Cannot statically analyze what the element type is of the object collection in type so its members may be trimmed.")]
    private static object BindToCollection(Type type, IConfiguration config, ExtendedBinderOptions options)
    {
        Type genericType = typeof(List<>).MakeGenericType(type.GenericTypeArguments[0]);
        object instance = Activator.CreateInstance(genericType);
        BindCollection(instance, genericType, config, options);
        return instance;
    }

    // Try to create an array/dictionary instance to back various collection interfaces
    [RequiresUnreferencedCode(
        "In case type is a Dictionary, cannot statically analyze what the element type is of the value objects in the dictionary so its members may be trimmed.")]
    private static object AttemptBindToCollectionInterfaces(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type type,
        IConfiguration config, ExtendedBinderOptions options)
    {
        if (!type.IsInterface)
        {
            return null;
        }

        Type collectionInterface = FindOpenGenericInterface(typeof(IReadOnlyList<>), type);
        if (collectionInterface != null)
        {
            // IEnumerable<T> is guaranteed to have exactly one parameter
            return BindToCollection(type, config, options);
        }

        collectionInterface = FindOpenGenericInterface(typeof(IReadOnlyDictionary<,>), type);
        if (collectionInterface != null)
        {
            Type dictionaryType =
                typeof(Dictionary<,>).MakeGenericType(type.GenericTypeArguments[0], type.GenericTypeArguments[1]);
            object instance = Activator.CreateInstance(dictionaryType);
            BindDictionary(instance, dictionaryType, config, options);
            return instance;
        }

        collectionInterface = FindOpenGenericInterface(typeof(IDictionary<,>), type);
        if (collectionInterface != null)
        {
            object instance = Activator.CreateInstance(
                typeof(Dictionary<,>).MakeGenericType(type.GenericTypeArguments[0], type.GenericTypeArguments[1]));
            BindDictionary(instance, collectionInterface, config, options);
            return instance;
        }

        collectionInterface = FindOpenGenericInterface(typeof(IReadOnlyCollection<>), type);
        if (collectionInterface != null)
        {
            // IReadOnlyCollection<T> is guaranteed to have exactly one parameter
            return BindToCollection(type, config, options);
        }

        collectionInterface = FindOpenGenericInterface(typeof(ICollection<>), type);
        if (collectionInterface != null)
        {
            // ICollection<T> is guaranteed to have exactly one parameter
            return BindToCollection(type, config, options);
        }

        collectionInterface = FindOpenGenericInterface(typeof(IEnumerable<>), type);
        if (collectionInterface != null)
        {
            // IEnumerable<T> is guaranteed to have exactly one parameter
            return BindToCollection(type, config, options);
        }

        return null;
    }

    [RequiresUnreferencedCode(TrimmingWarningMessage)]
    private static object BindInstance(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type type,
        object instance, IConfiguration config, ExtendedBinderOptions options,
        out bool instanceIsLeafAndNotFoundedInConfig)
    {
        instanceIsLeafAndNotFoundedInConfig = true;
        // if binding IConfigurationSection, break early
        if (type == typeof(IConfigurationSection))
        {
            return config;
        }

        var section = config as IConfigurationSection;
        string configValue = section?.Value;
        object convertedValue;
        Exception error;
        if (configValue != null && TryConvertValue(type, configValue, section.Path, out convertedValue, out error))
        {
            instanceIsLeafAndNotFoundedInConfig = false;
            if (error != null)
            {
                throw error;
            }

            // Leaf nodes are always reinitialized
            return convertedValue;
        }

        if (config != null && config.GetChildren().Any())
        {
            // If we don't have an instance, try to create one
            if (instance == null)
            {
                // We are already done if binding to a new collection instance worked
                instance = AttemptBindToCollectionInterfaces(type, config, options);
                if (instance != null)
                {
                    return instance;
                }

                instance = CreateInstance(type);
            }

            // See if its a Dictionary
            Type collectionInterface = FindOpenGenericInterface(typeof(IDictionary<,>), type);
            if (collectionInterface != null)
            {
                BindDictionary(instance, collectionInterface, config, options);
            }
            else if (type.IsArray)
            {
                instance = BindArray((Array) instance, config, options);
            }
            else
            {
                // See if its an ICollection
                collectionInterface = FindOpenGenericInterface(typeof(ICollection<>), type);
                if (collectionInterface != null)
                {
                    BindCollection(instance, collectionInterface, config, options);
                }
                // Something else
                else
                {
                    BindNonScalar(config, instance, options);
                }
            }
        }

        return instance;
    }

    private static object CreateInstance(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors |
                                    DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        Type type)
    {
        if (type.IsInterface || type.IsAbstract)
        {
            throw new InvalidOperationException("Error_CannotActivateAbstractOrInterface");
        }

        if (type.IsArray)
        {
            if (type.GetArrayRank() > 1)
            {
                throw new InvalidOperationException("Error_UnsupportedMultidimensionalArray");
            }

            return Array.CreateInstance(type.GetElementType(), 0);
        }

        if (!type.IsValueType)
        {
            bool hasDefaultConstructor = type.GetConstructors(DeclaredOnlyLookup)
                .Any(ctor => ctor.IsPublic && ctor.GetParameters().Length == 0);
            if (!hasDefaultConstructor)
            {
                throw new InvalidOperationException("Error_MissingParameterlessConstructor");
            }
        }

        try
        {
            return Activator.CreateInstance(type);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error_FailedToActivate", ex);
        }
    }

    [RequiresUnreferencedCode(
        "Cannot statically analyze what the element type is of the value objects in the dictionary so its members may be trimmed.")]
    private static void BindDictionary(
        object dictionary,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type dictionaryType,
        IConfiguration config, ExtendedBinderOptions options)
    {
        // IDictionary<K,V> is guaranteed to have exactly two parameters
        Type keyType = dictionaryType.GenericTypeArguments[0];
        Type valueType = dictionaryType.GenericTypeArguments[1];
        bool keyTypeIsEnum = keyType.IsEnum;

        if (keyType != typeof(string) && !keyTypeIsEnum)
        {
            // We only support string and enum keys
            return;
        }

        PropertyInfo setter = dictionaryType.GetProperty("Item", DeclaredOnlyLookup);
        foreach (IConfigurationSection child in config.GetChildren())
        {
            object item = BindInstance(
                type: valueType,
                instance: null,
                config: child,
                options: options,
                out _);
            if (item != null)
            {
                if (keyType == typeof(string))
                {
                    string key = child.Key;
                    setter.SetValue(dictionary, item, new object[] {key});
                }
                else if (keyTypeIsEnum)
                {
                    object key = Enum.Parse(keyType, child.Key);
                    setter.SetValue(dictionary, item, new[] {key});
                }
            }
        }
    }

    [RequiresUnreferencedCode(
        "Cannot statically analyze what the element type is of the object collection so its members may be trimmed.")]
    private static void BindCollection(
        object collection,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                                    DynamicallyAccessedMemberTypes.NonPublicMethods)]
        Type collectionType,
        IConfiguration config, ExtendedBinderOptions options)
    {
        // ICollection<T> is guaranteed to have exactly one parameter
        Type itemType = collectionType.GenericTypeArguments[0];
        MethodInfo addMethod = collectionType.GetMethod("Add", DeclaredOnlyLookup);

        foreach (IConfigurationSection section in config.GetChildren())
        {
            try
            {
                object item = BindInstance(
                    type: itemType,
                    instance: null,
                    config: section,
                    options: options,
                    out _);
                if (item != null)
                {
                    addMethod.Invoke(collection, new[] {item});
                }
            }
            catch
            {
            }
        }
    }

    [RequiresUnreferencedCode(
        "Cannot statically analyze what the element type is of the Array so its members may be trimmed.")]
    private static Array BindArray(Array source, IConfiguration config, ExtendedBinderOptions options)
    {
        IConfigurationSection[] children = config.GetChildren().ToArray();
        int arrayLength = source.Length;
        Type elementType = source.GetType().GetElementType();
        var newArray = Array.CreateInstance(elementType, arrayLength + children.Length);

        // binding to array has to preserve already initialized arrays with values
        if (arrayLength > 0)
        {
            Array.Copy(source, newArray, arrayLength);
        }

        for (int i = 0; i < children.Length; i++)
        {
            try
            {
                object item = BindInstance(
                    type: elementType,
                    instance: null,
                    config: children[i],
                    options: options,
                    out _);
                if (item != null)
                {
                    newArray.SetValue(item, arrayLength + i);
                }
            }
            catch
            {
            }
        }

        return newArray;
    }

    [RequiresUnreferencedCode(TrimmingWarningMessage)]
    private static bool TryConvertValue(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type type,
        string value, string path, out object result, out Exception error)
    {
        error = null;
        result = null;
        if (type == typeof(object))
        {
            result = value;
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            return TryConvertValue(Nullable.GetUnderlyingType(type), value, path, out result, out error);
        }

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        if (converter.CanConvertFrom(typeof(string)))
        {
            try
            {
                result = converter.ConvertFromInvariantString(value);
            }
            catch (Exception ex)
            {
                error = new InvalidOperationException("Error_FailedBinding", ex);
            }

            return true;
        }

        if (type == typeof(byte[]))
        {
            try
            {
                result = Convert.FromBase64String(value);
            }
            catch (FormatException ex)
            {
                error = new InvalidOperationException("Error_FailedBinding", ex);
            }

            return true;
        }

        return false;
    }

    [RequiresUnreferencedCode(TrimmingWarningMessage)]
    private static object ConvertValue(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type type,
        string value, string path)
    {
        object result;
        Exception error;
        TryConvertValue(type, value, path, out result, out error);
        if (error != null)
        {
            throw error;
        }

        return result;
    }

    private static Type FindOpenGenericInterface(
        Type expected,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
        Type actual)
    {
        if (actual.IsGenericType &&
            actual.GetGenericTypeDefinition() == expected)
        {
            return actual;
        }

        Type[] interfaces = actual.GetInterfaces();
        foreach (Type interfaceType in interfaces)
        {
            if (interfaceType.IsGenericType &&
                interfaceType.GetGenericTypeDefinition() == expected)
            {
                return interfaceType;
            }
        }

        return null;
    }

    private static List<PropertyInfo> GetAllProperties(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type type)
    {
        var allProperties = new List<PropertyInfo>();

        do
        {
            allProperties.AddRange(type.GetProperties(DeclaredOnlyLookup));
            type = type.BaseType;
        } while (type != typeof(object));

        return allProperties;
    }

    [RequiresUnreferencedCode(PropertyTrimmingWarningMessage)]
    private static object GetPropertyValue(PropertyInfo property, object instance,
        IConfiguration config,
        ExtendedBinderOptions options)
    {
        string propertyName = GetPropertyName(property);

        var bindSuccessResult = BindInstance(
            property.PropertyType,
            property.GetValue(instance),
            config.GetSection(propertyName),
            options,
            out var instanceIsLeafAndNotFoundedInConfig);

        if (!instanceIsLeafAndNotFoundedInConfig) return bindSuccessResult;

        _ = options.CasesSupport.ClassOptionToTargetOptionCaseProjectors
            .SkipWhile(propertyNameProjector =>
            {
                bindSuccessResult = BindInstance(
                    property.PropertyType,
                    property.GetValue(instance),
                    config.GetSection(propertyNameProjector(property.Name)),
                    options,
                    out instanceIsLeafAndNotFoundedInConfig);

                return instanceIsLeafAndNotFoundedInConfig;
            })
            .ToArray();

        return bindSuccessResult;
    }

    private static string GetPropertyName(MemberInfo property)
    {
        if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        // Check for a custom property name used for configuration key binding
        foreach (var attributeData in property.GetCustomAttributesData())
        {
            if (attributeData.AttributeType != typeof(ConfigurationKeyNameAttribute))
            {
                continue;
            }

            // Ensure ConfigurationKeyName constructor signature matches expectations
            if (attributeData.ConstructorArguments.Count != 1)
            {
                break;
            }

            // Assumes ConfigurationKeyName constructor first arg is the string key name
            string name = attributeData
                .ConstructorArguments[0]
                .Value?
                .ToString();

            return !string.IsNullOrWhiteSpace(name) ? name : property.Name;
        }

        return property.Name;
    }
}