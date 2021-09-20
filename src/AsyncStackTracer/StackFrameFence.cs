using System;
using System.Runtime.CompilerServices;

namespace AsyncStackTracer
{
    class StackFrameFenceBegin
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Invoke(Action action) => action();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Invoke<T>(Action<T> action, T state) => action(state);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Invoke<T>(Func<T> action) => action();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TResult Invoke<TState, TResult>(Func<TState, TResult> action, TState state) => action(state);
    }

    class StackFrameFenceEnd
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Invoke(Action action) => action();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Invoke<T>(Action<T> action, T state) => action(state);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Invoke<T>(Func<T> action) => action();

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static TResult Invoke<TState, TResult>(Func<TState, TResult> action, TState state) => action(state);
    }
}
