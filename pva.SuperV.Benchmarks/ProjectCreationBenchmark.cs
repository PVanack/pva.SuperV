﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using pva.SuperV.Engine;

namespace pva.SuperV.Benchmarks
{
    /// <summary>Benchmark on project creation with different number of classes and fields.</summary>
    [SimpleJob(launchCount: 1, warmupCount: 1, iterationCount: 1)]
    public class ProjectCreationBenchmark
    {
        [Params(100, 200)]
        public int ClassesCount { get; set; }

        [Params(10, 50)]
        public int FieldsCount { get; set; }

        [Benchmark]
        public void CreateProjectWithClasses()
        {
            WipProject theProject = Project.CreateProject(BenchmarkHelpers.ProjectName);
            for (int classIndex = 0; classIndex < ClassesCount; classIndex++)
            {
                string className = $"{BenchmarkHelpers.ClassName}_{classIndex}";
                _ = theProject.AddClass(className);
                for (int fieldIndex = 0; fieldIndex < FieldsCount; fieldIndex++)
                {
                    theProject.AddField(className, new FieldDefinition<int>(String.Format($"{BenchmarkHelpers.FieldName}_{fieldIndex}"), fieldIndex));
                }
            }
            _ = Task.Run(async () => await Project.BuildAsync(theProject)).Result;
        }
    }

    /// <summary>Benchmark on instances creation.</summary>
    [SimpleJob(RunStrategy.Throughput)]
    public class InstanceCreation
    {
        private readonly RunnableProject runnableProject = SetupForCreateInstances();

        public static RunnableProject SetupForCreateInstances()
        {
            WipProject wipProject = Project.CreateProject(BenchmarkHelpers.ProjectName);
            _ = wipProject.AddClass(BenchmarkHelpers.ClassName);
            wipProject.AddField(BenchmarkHelpers.ClassName, new FieldDefinition<int>(BenchmarkHelpers.FieldName, 10));
            return Task.Run(async () => await Project.BuildAsync(wipProject)).Result;
        }

        [Params(100, 200)]
        public int InstancesCount { get; set; }

        [Benchmark]
        public void CreateInstances()
        {
            for (int index = 0; index < InstancesCount; index++)
            {
                runnableProject.CreateInstance(BenchmarkHelpers.ClassName, String.Format($"{BenchmarkHelpers.InstanceName}-{index}"));
            }
        }
    }
}