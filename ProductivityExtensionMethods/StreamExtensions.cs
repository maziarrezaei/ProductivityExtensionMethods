#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System.CodeDom.Compiler;

namespace System.IO
{
    [GeneratedCode("ProductivityExtensionMethods", "VersionPlaceholder{D8B1B561-500C-4086-91AA-0714457205DA}")]
    public static partial class StreamExtensions
    {
        #region  Public Methods

        /// <summary>
        ///     Copies an stream to another stream using the buffer passed.
        /// </summary>
        /// <param name="source">source stream</param>
        /// <param name="destination">destination stream</param>
        /// <param name="buffer">buffer to use during copy</param>
        /// <returns>number of bytes copied</returns>
        public static long CopyTo(this Stream source, Stream destination, byte[] buffer)
        {
            long total = 0;
            int bytesRead;

            while (true)
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    return total;

                total += bytesRead;
                destination.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        ///     Ensures that the specified number of bytes are read from a stream unless it reaches its end.
        /// </summary>
        /// <param name="source">The stream to read from.</param>
        /// <param name="buffer">
        ///     An array of bytes. When this method returns, the buffer contains the specified
        ///     byte array with the values between offset and (offset + count - 1) replaced
        ///     by the bytes read from the current source.
        /// </param>
        /// <param name="offset">
        ///     The zero-based byte offset in buffer at which to begin storing the data read
        ///     from the current stream.
        /// </param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        ///     The total number of bytes read into the buffer. This is equal to the
        ///     number of bytes requested or less if the end of the stream has been reached.
        /// </returns>
        public static int SafeRead(this Stream source, byte[] buffer, int offset, int count)
        {
            int t;
            int total = 0;

            while (count > 0)
            {
                t = source.Read(buffer, offset, count);
                if (t == 0)
                    break;

                total += t;
                offset += t;
                count -= t;
            }

            return total;
        }

        #endregion
    }
}