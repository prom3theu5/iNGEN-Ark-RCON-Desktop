using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PTK.Utils
{
    public class Benchmark: IDisposable
    {
        private Stopwatch Stopwatch {get; set;}
        private string Name {get; set;}

        public Benchmark([CallerMemberName] string name = "")
        {
            Name = name;
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }

        public void Dispose()
        {
            Stopwatch.Stop();
            Console.WriteLine("{0}: {1}ms", Name, Stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}
