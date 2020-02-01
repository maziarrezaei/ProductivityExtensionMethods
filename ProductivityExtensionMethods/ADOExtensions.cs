#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

namespace System.Data
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;

    [GeneratedCode("ProductivityExtensionMethods", "VersionPlaceholder{D8B1B561-500C-4086-91AA-0714457205DA}")]
    public static partial class ADOExtensions
    {
        #region ADO Extensions

        public static IEnumerable<T> AsEnumerable<T>(this T reader, bool disposeReader = true) where T : IDataReader
        {
            try
            {
                while (reader.Read())
                    yield return reader;
            }
            finally
            {
                if (disposeReader)
                    reader.Close();
            }
        }

        #endregion
    }
}
