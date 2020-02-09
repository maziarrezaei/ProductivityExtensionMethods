#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System;
using System.Collections.Concurrent;
using System.Reflection;

public static partial class ProductivityExtensions
{
    private static readonly ConcurrentDictionary<(Type closedType, Type openType), Type[]?> IsAClosedTypeOfCache = new ConcurrentDictionary<(Type closedType, Type openType), Type[]?>();

    /// <summary>
    ///     Checks if a type is assignable to a certain generic type with no need to specify type parameters. For example, if a
    ///     type is IDictionary<,>
    /// </summary>
    /// <remarks>
    ///     For the purposes of this method, <paramref name="type" /> is considered a closed type of the non-constructed (i.e.
    ///     open) <paramref name="openGenericType" />, if an instance of <paramref name="type" /> can be casted to a closed
    ///     type created from <paramref name="openGenericType" /> with the same type-parameters that was used to construct
    ///     <paramref name="type" /> or one if its base classes/interfaces.
    ///     This is true when a type or one of its base classes implements or inherits from <paramref name="openGenericType" />
    ///     . For example, a class that implements generic IList and is a now closed type is considered a closed type of it,
    ///     since it is assignable to generic IList
    ///     Refer to
    ///     <see cref="https://docs.microsoft.com/en-us/dotnet/framework/reflection-and-codedom/reflection-and-generic-types" />
    ///     for detailed definition of open and closed generic types.
    /// </remarks>
    /// <param name="type">
    ///     Any type can be passed to be examined, however the type must be a closed generic type, or a non
    ///     generic type.
    /// </param>
    /// <param name="openGenericType">
    ///     A generic type, without any type parameters specified. A rule of thumb for such type is
    ///     that one should be able to create a closed type of it by only providing a number of type parameters.
    /// </param>
    /// <param name="genericTypeParameters">
    ///     If <paramref name="type" /> is a closed type of <paramref name="openGenericType" />
    ///     , the parameters that was used to construct the generic <paramref name="type" /> or one of its base
    ///     classes/interfaces
    /// </param>
    /// <returns>
    ///     True, If an instance of <paramref name="type" /> can be casted to a closed type created from
    ///     <paramref name="openGenericType" /> with the same generic type parameters that was used to defining
    ///     <paramref name="type" />. Otherwise, false.
    /// </returns>
    public static bool IsAClosedTypeOf(this Type type, Type openGenericType, out Type[]? genericTypeParameters)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (openGenericType == null)
            throw new ArgumentNullException(nameof(openGenericType));

        if (type.IsGenericTypeDefinition || type.ContainsGenericParameters)
            throw new ArgumentException("All generic parameters must be assigned and each generic parameter must be a non generic type or a closed generic type. Supplied type:" + type, nameof(type));

        if (!openGenericType.ContainsGenericParameters)
            throw new ArgumentException($"Parameter supplied does not refer to an open generic type definition. Supplied type: {openGenericType} ", nameof(openGenericType));

        if (!openGenericType.IsGenericTypeDefinition)
            throw new ArgumentException($"Parameter supplied does not refer to a generic type definition. Though the type {openGenericType} is an open type, but it not directly closable by suppling a pre-defined list of type parameters." + "For example, you can never supply type parameters to type.MakeGenericType(...) when the type is typeof(Dictionary<string,List<>>). Such" + "types can only be created through reflection and no language allows creation of such types using literal type specification such as the example.", nameof(openGenericType));

        (Type type, Type openGenericType) keyToFind = (type, openGenericType);

        genericTypeParameters = IsAClosedTypeOfCache.GetOrAdd(keyToFind, (key, arg) =>
                                                                         {
                                                                             if (key.openType.FullName == null)
                                                                                 throw new ArgumentException("The instance represents a generic type parameter, an array type, pointer type, or byref type based on a type parameter", nameof(openGenericType));

                                                                             if (key.openType.IsInterface)
                                                                             {
                                                                                 Type? ty = key.closedType.GetInterface(key.openType.FullName);

                                                                                 if (ty != null)
                                                                                     return ty.GetGenericArguments();

                                                                                     // The .GetInterface() does not handle simple cases like IDic<string,string> against IDic<,>, so we'll let it go through
                                                                                     // regular type check as if it's not an interface
                                                                                 }

                                                                             Type[] genericArgsOfType;
                                                                             Type? currentBaseType = key.closedType;

                                                                             while (true)
                                                                             {
                                                                                 while (!currentBaseType.IsGenericType)
                                                                                 {
                                                                                     currentBaseType = currentBaseType.BaseType;

                                                                                     if (currentBaseType == null || currentBaseType == typeof(object))
                                                                                         return null;
                                                                                 }

                                                                                 genericArgsOfType = currentBaseType.GetGenericArguments();

                                                                                 if (key.openType.TryMakeGenericType(out Type? newGenType, genericArgsOfType) && newGenType != null && newGenType.IsAssignableFrom(currentBaseType))
                                                                                     return genericArgsOfType;
                                                                             }
                                                                         }, default(object));

        return genericTypeParameters != null;
    }

    /// <summary>
    ///     A version of Type.MakeGenericType that does not throw exception.
    /// </summary>
    /// <param name="genType">The type to construct a generic type from.</param>
    /// <param name="resultType">The constructed generic type.</param>
    /// <param name="typePrms">The type parameters</param>
    /// <returns></returns>
    public static bool TryMakeGenericType(this Type genType, out Type? resultType, params Type[] typePrms)
    {
        resultType = null;

        try
        {
            if (genType.GetGenericArguments().Length != typePrms.Length)
                return false;

            resultType = genType.MakeGenericType(typePrms);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool HasAttribute<T>(this MemberInfo mi) where T : Attribute
    {
        return Attribute.GetCustomAttribute(mi, typeof(T), true) != null;
    }
}
