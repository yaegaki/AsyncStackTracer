using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AsyncStackTracer
{
    public static class AsyncStackTraceHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static AsyncStackTraceContext CreateContext(int skipFrames = 0)
        {
            var connector = SimpleStackFrameConnector.Default;
            // skip AsyncStackTraceHelper.CreateContext
            skipFrames = skipFrames + 1;
            return StackFrameFenceEnd.Invoke(x => new AsyncStackTraceContext(x.connector, x.skipFrames), (connector, skipFrames));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static AsyncStackTraceContext CreateContext(IStackFrameConnector connector, int skipFrames = 0)
        {
            connector = connector ?? SimpleStackFrameConnector.Default;
            // skip AsyncStackTraceHelper.CreateContext
            skipFrames = skipFrames + 1;
            return StackFrameFenceEnd.Invoke(x => new AsyncStackTraceContext(x.connector, x.skipFrames), (connector, skipFrames));
        }

        public static bool IsBeginFence(AsyncStackFrame stackFrame)
            => IsFence<StackFrameFenceBegin>(stackFrame.StackFrame);

        public static bool IsBeginFence(StackFrame stackFrame)
            => IsFence<StackFrameFenceBegin>(stackFrame);

        public static bool IsEndFence(AsyncStackFrame stackFrame)
            => IsFence<StackFrameFenceEnd>(stackFrame.StackFrame);

        public static bool IsEndFence(StackFrame stackFrame)
            => IsFence<StackFrameFenceEnd>(stackFrame);

        private static bool IsFence<T>(StackFrame stackFrame)
        {
            if (stackFrame == null) return false;
            var method = stackFrame.GetMethod();
            return method.DeclaringType == typeof(T);
        }
    }
}
