using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AsyncStackTracer.Samples.SimpleAction
{
    class Program
    {
        private static readonly List<Action> actions = new List<Action>();

        static void Main(string[] args)
        {
            Hoge();

            // invoke all asynchronous actions
            foreach (var action in actions)
            {
                action();
            }
        }

        static void Hoge()
        {
            Console.WriteLine("--- call synchronous ---");
            Console.WriteLine("StackTrace:");
            Console.WriteLine(new StackTrace(true));

            AsyncInvoke(DoWithOriginalStackTrace);

            AsyncInvoke(AsyncStackTraceHelper.CreateContext().Wrap(DoWithAsyncStackTrace));
        }

        static void DoWithOriginalStackTrace()
        {
            Console.WriteLine("--- call asynchronous(original stacktrace) ---");
            Console.WriteLine("StackTrace:");
            Console.WriteLine(new StackTrace(true));
        }

        static void DoWithAsyncStackTrace()
        {
            Console.WriteLine("--- call asynchronous(async stacktrace) ---");
            Console.WriteLine("StackTrace:");
            Console.WriteLine(new AsyncStackTrace(true));
        }

        static void AsyncInvoke(Action action)
            => actions.Add(action);
    }
}
