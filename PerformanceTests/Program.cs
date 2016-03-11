using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Linq;

namespace PerformanceTests
{
    public class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Program>();
        }

        [Benchmark]
        public void Benchmark()
        {
            Solution3.Program.Main(Enumerable.Empty<string>().ToArray());
        }
    }
}
