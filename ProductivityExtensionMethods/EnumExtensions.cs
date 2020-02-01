#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System.CodeDom.Compiler;

namespace System
{
    [GeneratedCode("ProductivityExtensionMethods", "VersionPlaceholder{D8B1B561-500C-4086-91AA-0714457205DA}")]
    public static partial class EnumExtensions
    {
        #region  Public Methods

        public static T ToEnumChecked<T>(this int v) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), v))
                return (T)Enum.ToObject(typeof(T), v);

            throw new InvalidCastException($"Cannot cast value {v} to enum type {typeof(T)}");
        }

        public static bool TryToEnum<T>(this int v, out T e) where T : Enum
        {
            Type t = typeof(T);

            if (!Enum.IsDefined(t, v))
            {
                e = (T)Enum.GetValues(t).GetValue(0);
                return false;
            }

            e = (T)Enum.ToObject(t, v);
            return true;
        }

        #endregion
    }
}