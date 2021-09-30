using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AsyncStackTracer
{
    public static class AsyncStackTraceHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static AsyncStackTraceContext CreateContext(int skipFrames = 0)
        {
            var connector = SimpleStackTraceMerger.Instance;
            return new AsyncStackTraceContext(connector, skipFrames + 1);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static AsyncStackTraceContext CreateContext(IStackTraceMerger connector, int skipFrames = 0)
        {
            connector = connector ?? SimpleStackTraceMerger.Instance;
            return new AsyncStackTraceContext(connector, skipFrames + 1);
        }

        public static bool IsFence(StackFrame stackFrame)
        {
            if (stackFrame == null) return false;
            var method = stackFrame.GetMethod();
            return method.DeclaringType == typeof(StackFrameFence);
        }
    }
}
