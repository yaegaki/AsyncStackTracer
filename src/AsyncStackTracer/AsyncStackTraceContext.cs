using System;
using System.Threading;

namespace AsyncStackTracer
{
    public readonly struct AsyncStackTraceContext
    {
        internal static readonly ThreadLocal<AsyncStackTraceContext> Current = new ThreadLocal<AsyncStackTraceContext>();

        internal readonly AsyncStackTrace stackTrace;
        internal readonly IStackTraceMerger stackTraceMerger;

        internal bool IsEmpty => stackTrace == null;

        internal AsyncStackTraceContext(IStackTraceMerger stackTraceMerger, int skipFrames)
        {
            stackTrace = new AsyncStackTrace(skipFrames + 1, false);
            this.stackTraceMerger = stackTraceMerger;
        }

        public Action Wrap(Action action)
        {
            var c = this;
            return () => c.Use(action);
        }

        public Action<T> Wrap<T>(Action<T> action)
        {
            var c = this;
            return x => c.Use(action, x);
        }

        public Func<T> Wrap<T>(Func<T> action)
        {
            var c = this;
            return () => c.Use(action);
        }

        public Func<TState, TResult> Wrap<TState, TResult>(Func<TState, TResult> action)
        {
            var c = this;
            return x => c.Use(action, x);
        }

        public void Use(Action action)
        {
            var current = AsyncStackTraceContext.Current.Value;
            try
            {
                AsyncStackTraceContext.Current.Value = this;
                StackFrameFence.Invoke(action);
            }
            finally
            {
                AsyncStackTraceContext.Current.Value = current;
            }
        }

        public void Use<T>(Action<T> action, T state)
        {
            var current = AsyncStackTraceContext.Current.Value;
            try
            {
                AsyncStackTraceContext.Current.Value = this;
                StackFrameFence.Invoke(action, state);
            }
            finally
            {
                AsyncStackTraceContext.Current.Value = current;
            }
        }

        public T Use<T>(Func<T> action)
        {
            var current = AsyncStackTraceContext.Current.Value;
            try
            {
                AsyncStackTraceContext.Current.Value = this;
                return StackFrameFence.Invoke(action);
            }
            finally
            {
                AsyncStackTraceContext.Current.Value = current;
            }
        }

        public TResult Use<TState, TResult>(Func<TState, TResult> action, TState state)
        {
            var current = AsyncStackTraceContext.Current.Value;
            try
            {
                AsyncStackTraceContext.Current.Value = this;
                return StackFrameFence.Invoke(action, state);
            }
            finally
            {
                AsyncStackTraceContext.Current.Value = current;
            }
        }
    }
}
