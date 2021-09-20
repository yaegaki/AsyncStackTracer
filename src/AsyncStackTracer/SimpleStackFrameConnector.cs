using System.Collections.Generic;
using System.Diagnostics;

namespace AsyncStackTracer
{
    public class SimpleStackFrameConnector : IStackFrameConnector
    {
        public static int DefaultStackFrameCountLimit = 200;
        public static readonly SimpleStackFrameConnector Default = new SimpleStackFrameConnector(DefaultStackFrameCountLimit, false);
        public static readonly SimpleStackFrameConnector IncludeAll = new SimpleStackFrameConnector(DefaultStackFrameCountLimit, true);

        private readonly int stackFrameCountLimit;
        private readonly bool includeAll;

        public SimpleStackFrameConnector(int stackFrameCountLimit, bool includeAll)
        {
            this.stackFrameCountLimit = stackFrameCountLimit;
            this.includeAll = includeAll;
        }

        public IReadOnlyList<AsyncStackFrame> Connect(IReadOnlyList<AsyncStackFrame> parentFrames, int parentSkipFrames, StackTrace currentStackTrace)
        {
            var capacity = currentStackTrace.FrameCount + parentFrames.Count;
            if (capacity > stackFrameCountLimit)
            {
                capacity = stackFrameCountLimit;
            }
            var result = new List<AsyncStackFrame>(capacity);

            for (var i = 0; i < currentStackTrace.FrameCount && result.Count < capacity; i++)
            {
                var frame = currentStackTrace.GetFrame(i);
                // skip frames after fence
                if (!includeAll && AsyncStackTraceHelper.IsBeginFence(frame)) break;
                result.Add(new AsyncStackFrame(currentStackTrace, frame));
            }

            var foundFence = false;

            for (var i = 0; i < parentFrames.Count && result.Count < capacity; i++)
            {
                var frame = parentFrames[i];
                if (!includeAll && !foundFence)
                {
                    if (AsyncStackTraceHelper.IsEndFence(frame))
                    {
                        foundFence = true;
                        i += parentSkipFrames;
                    }

                    continue;
                }
                result.Add(frame);
            }

            return result;
        }
    }
}
