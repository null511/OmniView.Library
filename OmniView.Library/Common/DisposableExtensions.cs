using System;

namespace OmniView.Library.Common
{
    internal static class DisposableExtensions
    {
        public static bool TryDispose(this IDisposable @object)
        {
            try {
                @object.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
