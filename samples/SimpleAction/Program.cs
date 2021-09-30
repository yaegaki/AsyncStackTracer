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
            InvokeAllActions();
        }

        static void Hoge()
        {
            Console.WriteLine("--- synchronous ---");
            Console.WriteLine("OriginalStackTrace:");
            Console.WriteLine(new StackTrace(true));

            AsyncInvoke(AsyncStackTraceHelper.CreateContext().Wrap(() =>
            {
                Console.WriteLine("--- asynchronous ---");
                Console.WriteLine("OriginalStackTrace:");
                Console.WriteLine(new StackTrace(true));
                Console.WriteLine("AsyncStackTrace:");
                Console.WriteLine(new AsyncStackTrace(true));

                AsyncInvoke(AsyncStackTraceHelper.CreateContext().Wrap(() =>
                {
                    Console.WriteLine("--- nested asynchronous ---");
                    Console.WriteLine("OriginalStackTrace:");
                    Console.WriteLine(new StackTrace(true));
                    Console.WriteLine("AsyncStackTrace:");
                    Console.WriteLine(new AsyncStackTrace(true));
                }));
            }));
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

        static void InvokeAllActions()
        {
            while (actions.Count > 0)
            {
                var temp = new List<Action>(actions);
                actions.Clear();
                foreach (var x in temp)
                {
                    x();
                }
            }
        }
    }
}
