using System.Diagnostics;
using System.Reflection;

namespace AsyncStackTracer
{
    public readonly struct AsyncStackFrame
    {
        public readonly StackTrace OriginalStackTrace;
        public readonly StackFrame StackFrame;

        public AsyncStackFrame(StackTrace originalStackTrace, StackFrame stackFrame)
        {
            OriginalStackTrace = originalStackTrace;
            StackFrame = stackFrame;
        }

        public MethodBase GetMethod() => StackFrame?.GetMethod();
    }
}
