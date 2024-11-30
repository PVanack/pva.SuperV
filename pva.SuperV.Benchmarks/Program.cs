using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using pva.SuperV.Builder;
using pva.SuperV.Model;

namespace pva.SuperV.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<ProjectCreationBenchmark>();
            BenchmarkRunner.Run<InstanceCreationBenchmark>();
        }
    }
}
