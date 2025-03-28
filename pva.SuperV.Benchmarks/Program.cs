﻿using BenchmarkDotNet.Running;

namespace pva.SuperV.Benchmarks
{
    public static class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<ListExtensionsBenchmarks>();
            BenchmarkRunner.Run<ProjectCreationBenchmark>();
            BenchmarkRunner.Run<InstanceCreationBenchmark>();
        }
    }
}