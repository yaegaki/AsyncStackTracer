using System.Collections.Generic;
using System.Diagnostics;

namespace AsyncStackTracer
{
    public interface IStackTraceMerger
    {
        IReadOnlyList<AsyncStackFrame> Merge(IReadOnlyList<AsyncStackFrame> parentStackFrames, StackTrace stackTrace, int skipFrames);
    }
}
