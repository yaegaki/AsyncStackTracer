using System.Collections.Generic;
using System.Diagnostics;

namespace AsyncStackTracer
{
    public class SimpleStackTraceMerger : IStackTraceMerger
    {
        public static readonly SimpleStackTraceMerger Instance = new SimpleStackTraceMerger();

        public IReadOnlyList<AsyncStackFrame> Merge(IReadOnlyList<AsyncStackFrame> parentStackFrames, StackTrace stackTrace, int skipFrames)
        {
            var stackFrames = new List<AsyncStackFrame>();
            for (var i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);
                if (AsyncStackTraceHelper.IsFence(frame))
                {
                    break;
                }

                if (i < skipFrames) continue;
                stackFrames.Add(new AsyncStackFrame(stackTrace, frame));
            }

            return new ConnectedList<AsyncStackFrame>(stackFrames, parentStackFrames);
        }
    }
}
