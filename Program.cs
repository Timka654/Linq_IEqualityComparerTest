using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {

            var obj = new MyClass();

            var lobj = new List<MyClass>() { obj, obj, obj, obj, obj };

            Test(lobj, obj, suf: "List");

            var qobj = new Queue<MyClass>(lobj);

            Test(qobj, obj, suf: "Queue");

            var cqobj = new ConcurrentQueue<MyClass>(lobj);

            Test(cqobj, obj, suf: "ConcurrentQueue");
            Console.ReadLine();
        }


        static void Test(IEnumerable<MyClass> lobj, MyClass obj, int iter = 10_000, string suf = "")
        {
            var comparer = new MyClassComparer();
            //cache
            for (int i = 0; i < 10_000; i++)
            {
                lobj.Contains(obj);
                lobj.Contains(obj, comparer);
            }

            List<TimeSpan> results = new List<TimeSpan>();

            Stopwatch sw = new Stopwatch();

            for (int q = 0; q < 60; q++)
            {

                for (int i = 0; i < iter; i++)
                {
                    sw.Start();

                    lobj.Contains(obj);

                    sw.Stop();
                }

                results.Add(sw.Elapsed);

                sw.Reset();
            }

            Console.WriteLine($"default {suf} {results.Average(x=>x.TotalMilliseconds)}");

            results.Clear();

            for (int q = 0; q < 60; q++)
            {
                for (int i = 0; i < iter; i++)
                {
                    sw.Start();

                    lobj.Contains(obj, comparer);

                    sw.Stop();
                }
                results.Add(sw.Elapsed);

                sw.Reset();
            }

            Console.WriteLine($"custom {suf} {results.Average(x => x.TotalMilliseconds)}");
        }

    }
    public class MyClass
    {
        public int id { get; set; }
        public int id2 { get; set; }
    }

    public class MyClassComparer : IEqualityComparer<MyClass>
    {
        public bool Equals(MyClass x, MyClass y)
        {
            return x.Equals(y);
        }

        public int GetHashCode([DisallowNull] MyClass obj)
        {
            return obj.GetHashCode();
        }
    }
}
