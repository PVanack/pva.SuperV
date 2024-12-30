using BenchmarkDotNet.Attributes;

namespace pva.SuperV.Benchmarks
{
    [SimpleJob(launchCount: 1, warmupCount: 10, iterationCount: 10)]
    public class ListExtensionsBenchmarks
    {
        private readonly List<string> strings = [];
        private string temp = "";

        public ListExtensionsBenchmarks()
        {
            for (var i = 0; i < 1_000; i++)
            {
                strings.Add($"String{i}");
            }
        }

        [Benchmark]
        public void ForeachLoop()
        {
            foreach (var item in strings)
            {
                temp = $"{item}-A";
            }
        }

        [Benchmark]
        public void ForEachExtension()
        {
            strings.ForEach(item => temp = $"{item}-A");
        }
    }
}