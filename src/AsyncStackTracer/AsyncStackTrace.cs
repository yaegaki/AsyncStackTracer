using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AsyncStackTracer
{
    public class AsyncStackTrace
    {
        private readonly StackTrace originalStackTrace;
        private readonly bool fNeedFileInfo;
        private readonly IReadOnlyList<AsyncStackFrame> stackFrames;

        public int FrameCount => stackFrames.Count;
        public StackTrace OriginalStackTrace => originalStackTrace;

        public AsyncStackTrace()
            : this(1, false)
        {
        }

        public AsyncStackTrace(bool fNeedFileInfo)
            : this(1, fNeedFileInfo)
        {
        }

        public AsyncStackTrace(int skipFrames, bool fNeedFileInfo)
        {
            // always get all frames and file info
            originalStackTrace = new StackTrace(true);
            skipFrames += 1;
            this.fNeedFileInfo = fNeedFileInfo;

            var context = AsyncStackTraceContext.Current.Value;
            if (context.IsEmpty)
            {
                var count = originalStackTrace.FrameCount - skipFrames;
                if (count <= 0)
                {
                    stackFrames = Array.Empty<AsyncStackFrame>();
                }
                else
                {
                    var index = 0;
                    var array = new AsyncStackFrame[count];
                    for (var i = skipFrames; i < originalStackTrace.FrameCount; i++)
                    {
                        array[index] = new AsyncStackFrame(originalStackTrace, originalStackTrace.GetFrame(i));
                        index++;
                    }
                    stackFrames = array;
                }
            }
            else
            {
                stackFrames = context.stackTraceMerger.Merge(context.stackTrace.stackFrames, originalStackTrace, skipFrames);
            }
        }

        public IReadOnlyList<AsyncStackFrame> GetFrames()
            => stackFrames;

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var sf in stackFrames)
            {
                sb.Append("   at ");
                var mb = sf.StackFrame.GetMethod();
                sb.AppendFormat("{0}.{1}", mb.DeclaringType, mb.Name);
                if (fNeedFileInfo)
                {
                    sb.AppendFormat(" in {0}:{1}", sf.StackFrame.GetFileName(), sf.StackFrame.GetFileLineNumber());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
