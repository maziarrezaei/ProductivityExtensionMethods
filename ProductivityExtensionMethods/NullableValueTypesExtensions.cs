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
    public static partial class NullableValueTypesExtensions
    {
        #region  Public Methods

        public static bool HasValueAndEquals<T>(this T? source, object target) where T : struct
        {
            return source.HasValue && source.Value.Equals(target);
        }

        public static bool IsNullOrDefault<T>(this T? o) where T : struct
        {
            return !o.HasValue || o.Value.Equals(default(T));
        }

        #endregion
    }
}