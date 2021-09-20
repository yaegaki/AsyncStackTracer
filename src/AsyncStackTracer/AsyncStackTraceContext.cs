using System;
using System.Threading;

namespace AsyncStackTracer
{
    public readonly struct AsyncStackTraceContext
    {
        internal static readonly ThreadLocal<AsyncStackTraceContext> Current = new ThreadLocal<AsyncStackTraceContext>();

        internal readonly AsyncStackTrace stackTrace;
        internal readonly IStackFrameConnector stackFrameConnector;

        internal AsyncStackTraceContext(IStackFrameConnector stackFrameConnector, int skipFrames)
        {
            stackTrace = new AsyncStackTrace(skipFrames, false);
            this.stackFrameConnector = stackFrameConnector;
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
            try
            {
                AsyncStackTraceContext.Current.Value = this;
                StackFrameFenceBegin.Invoke(action);
            }
            finally
            {
                AsyncStackTraceContext.Current.Value = default;
            }
        }

        public void Use<T>(Action<T> action, T state)
        {
            try
            {
                AsyncStackTraceContext.Current.Value = this;
                StackFrameFenceBegin.Invoke(action, state);
            }
            finally
            {
                AsyncStackTraceContext.Current.Value = default;
            }
        }

        public T Use<T>(Func<T> action)
        {
            try
            {
                AsyncStackTraceContext.Current.Value = this;
                return StackFrameFenceBegin.Invoke(action);
            }
            finally
            {
                AsyncStackTraceContext.Current.Value = default;
            }
        }

        public TResult Use<TState, TResult>(Func<TState, TResult> action, TState state)
        {
            try
            {
                AsyncStackTraceContext.Current.Value = this;
                return StackFrameFenceBegin.Invoke(action, state);
            }
            finally
            {
                AsyncStackTraceContext.Current.Value = default;
            }
        }
    }
}
