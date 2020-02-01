#if (NETCOREAPP3_0 || NETCOREAPP3_1 || NETSTANDARD2_1)
#define SUPPORT_NETSTANDARD2_1_AND_ABOVE
#endif

#if (NETCOREAPP3_1 || NETCOREAPP3_0 || NETCOREAPP2_2 || NETCOREAPP2_1)
#define CORE2_1_AND_ABOVE
#endif

using System.CodeDom.Compiler;
using System.ComponentModel;

namespace System
{
    [GeneratedCode("ProductivityExtensionMethods", "VersionPlaceholder{D8B1B561-500C-4086-91AA-0714457205DA}")]
    public static partial class EventExtensions
    {
        #region  Public Methods

        public static void TryRaiseEventOnUIThread<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (eventHandler != null)
                foreach (EventHandler<TEventArgs> singleCast in eventHandler.GetInvocationList())
                {
                    if (singleCast.Target is ISynchronizeInvoke syncInvoke && syncInvoke.InvokeRequired)
                        // Invoke the event on the event subscribers thread
                        syncInvoke.Invoke(eventHandler, new[] {sender, e});
                    else
                        // Raise the event on this thread
                        singleCast(sender, e);
                }
        }

        public static void TryRaiseEventOnUIThread(this EventHandler eventHandler, object sender, EventArgs e)
        {
            if (eventHandler != null)
                foreach (EventHandler singleCast in eventHandler.GetInvocationList())
                {
                    if (singleCast.Target is ISynchronizeInvoke syncInvoke && syncInvoke.InvokeRequired)
                        // Invoke the event on the event subscribers thread
                        syncInvoke.Invoke(eventHandler, new[] {sender, e});
                    else
                        // Raise the event on this thread
                        singleCast(sender, e);
                }
        }

        #endregion
    }
}