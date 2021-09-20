using System.Collections.Generic;
using System.Diagnostics;

namespace AsyncStackTracer
{
    public interface IStackFrameConnector
    {
        IReadOnlyList<AsyncStackFrame> Connect(IReadOnlyList<AsyncStackFrame> parentFrames, int parentSkipFrames, StackTrace currentStackTrace);
    }
}
