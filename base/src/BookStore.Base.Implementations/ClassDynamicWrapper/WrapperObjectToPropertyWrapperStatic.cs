using System.Reflection;
using System.Reflection.Emit;

namespace BookStore.Base.Implementations.ClassDynamicWrapper;

public static class WrapperObjectToPropertyWrapperStatic
{
    private static readonly ModuleBuilder DynamicModuleBuilder;

    static WrapperObjectToPropertyWrapperStatic()
    {
        var wrappersDynamicAssembly = new AssemblyName("WrappersDynamicAssemblyStatic");

        var dynamicAssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
            wrappersDynamicAssembly,
            AssemblyBuilderAccess.Run);

        if (dynamicAssemblyBuilder == default)
        {
            throw new InvalidOperationException("Cannot dynamic create assembly");
        }

        DynamicModuleBuilder = dynamicAssemblyBuilder
            .DefineDynamicModule(wrappersDynamicAssembly.Name!);
    }

    public static object Wrap<TToWrap>(TToWrap objectToPropertyWrapper) where TToWrap : class
    {
        var dynamicTypeBuilder = DynamicModuleBuilder.DefineType(
            $"ConfigToPropertyWrapper_{Guid.NewGuid()}",
            TypeAttributes.Public);

        var inputType = typeof(TToWrap);

        var targetPropertyType = inputType;
        var targetPropertyName = inputType.Name;

        var targetField = dynamicTypeBuilder.DefineField(
            $"_{char.ToLower(targetPropertyName[0])}{targetPropertyName[1..]}",
            targetPropertyType,
            FieldAttributes.Private);

        CreateProperty(dynamicTypeBuilder, targetField, targetPropertyName,
            targetPropertyType);

        var resultType = dynamicTypeBuilder.CreateType();

        if (resultType == default)
        {
            throw new InvalidOperationException("Cannot dynamic create wrapper-type");
        }

        var resultTypeInstance = Activator.CreateInstance(resultType);

        if (resultTypeInstance == default)
        {
            throw new InvalidOperationException("Cannot create wrapper-type instance");
        }

        SetTargetObjectToWrapper(resultTypeInstance, targetPropertyName,
            objectToPropertyWrapper);

        return resultTypeInstance;
    }

    private static void SetTargetObjectToWrapper(object instance, string propertyName,
        object value)
    {
        var targetPropertyInObj = instance.GetType().GetProperty(propertyName);

        if (targetPropertyInObj == default)
        {
            throw new InvalidOperationException(
                $"Can't get property: [{propertyName}] from target instance");
        }

        targetPropertyInObj.SetValue(instance, value);
    }

    private static void CreateProperty(TypeBuilder dynamicTypeBuilder,
        FieldBuilder targetField, string targetPropertyName, Type targetPropertyType)
    {
        var targetProperty = dynamicTypeBuilder.DefineProperty(
            targetPropertyName,
            PropertyAttributes.HasDefault,
            targetPropertyType,
            Type.EmptyTypes);

        var getSetAttrs = MethodAttributes.Public
                          | MethodAttributes.SpecialName
                          | MethodAttributes.HideBySig;

        var targetPropertyGetAccessor = dynamicTypeBuilder.DefineMethod(
            $"get_{targetPropertyName}",
            getSetAttrs,
            targetPropertyType,
            Type.EmptyTypes);

        var targetPropertySetAccessor = dynamicTypeBuilder.DefineMethod(
            $"set_{targetPropertyName}",
            getSetAttrs,
            default,
            new[] {targetPropertyType});

        var targetPropertyGetAccessorIlGenerator = targetPropertyGetAccessor.GetILGenerator();
        targetPropertyGetAccessorIlGenerator.Emit(OpCodes.Ldarg_0);
        targetPropertyGetAccessorIlGenerator.Emit(OpCodes.Ldfld, targetField);
        targetPropertyGetAccessorIlGenerator.Emit(OpCodes.Ret);

        var targetPropertySetAccessorIlGenerator = targetPropertySetAccessor.GetILGenerator();
        targetPropertySetAccessorIlGenerator.Emit(OpCodes.Ldarg_0);
        targetPropertySetAccessorIlGenerator.Emit(OpCodes.Ldarg_1);
        targetPropertySetAccessorIlGenerator.Emit(OpCodes.Stfld, targetField);
        targetPropertySetAccessorIlGenerator.Emit(OpCodes.Ret);

        targetProperty.SetGetMethod(targetPropertyGetAccessor);
        targetProperty.SetSetMethod(targetPropertySetAccessor);
    }
}