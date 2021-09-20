using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AsyncStackTracer
{
    public class AsyncStackTrace
    {
        private readonly StackTrace originalStackTrace;
        private readonly IReadOnlyList<AsyncStackFrame> stackFrames;
        private readonly int skipFrames;
        private readonly int frameCount;
        private readonly bool fNeedFileInfo;

        public int FrameCount => frameCount;

        public AsyncStackTrace()
            : this(2, false)
        {
        }

        public AsyncStackTrace(bool fNeedFileInfo)
            : this(2, fNeedFileInfo)
        {
        }

        public AsyncStackTrace(int skipFrames, bool fNeedFileInfo)
        {
            // always get all frames and file info
            originalStackTrace = new StackTrace(true);
            this.skipFrames = skipFrames;
            this.fNeedFileInfo = fNeedFileInfo;

            var context = AsyncStackTraceContext.Current.Value;
            if (context.stackFrameConnector == null)
            {
                stackFrames = originalStackTrace.GetFrames()
                    .Select(x => new AsyncStackFrame(originalStackTrace, x))
                    .ToList();
            }
            else
            {
                var parent = context.stackTrace;
                stackFrames = context.stackFrameConnector.Connect(parent.stackFrames, parent.skipFrames, originalStackTrace);
            }

            frameCount = stackFrames.Count - this.skipFrames;
            if (frameCount < 0)
            {
                frameCount = 0;
            }
        }

        public StackTrace GetOriginalStackTrace()
        {
            return originalStackTrace;
        }

        public AsyncStackFrame GetFrame(int index)
        {
            index = index + skipFrames;
            if (index < 0 || index >= stackFrames.Count)
            {
                return default;
            }

            return stackFrames[index];
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var stackFrame in stackFrames.Skip(skipFrames).Select(f => f.StackFrame))
            {
                sb.Append("   at ");
                var mb = stackFrame.GetMethod();
                sb.AppendFormat("{0}.{1}", mb.DeclaringType, mb.Name);
                if (fNeedFileInfo)
                {
                    sb.AppendFormat(" in {0}:{1}", stackFrame.GetFileName(), stackFrame.GetFileLineNumber());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
