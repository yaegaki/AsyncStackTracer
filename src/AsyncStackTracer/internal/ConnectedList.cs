using System.Collections;
using System.Collections.Generic;

namespace AsyncStackTracer
{
    class ConnectedList<T> : IReadOnlyList<T>
    {
        private readonly IReadOnlyList<T> first;
        private readonly IReadOnlyList<T> second;

        public T this[int index]
        {
            get
            {
                var x = index - first.Count;
                if (x < 0)
                {
                    return first[index];
                }

                return second[x];
            }
        }

        public int Count { get; }

        public ConnectedList(IReadOnlyList<T> first, IReadOnlyList<T> second)
        {
            this.first = first;
            this.second = second;
            Count = first.Count + second.Count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var x in first)
            {
                yield return x;
            }

            foreach (var x in second)
            {
                yield return x;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
