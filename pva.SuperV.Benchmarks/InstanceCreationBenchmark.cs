using BenchmarkDotNet.Attributes;
using pva.SuperV.Engine;

namespace pva.SuperV.Benchmarks
{
    /// <summary>Benchmark on instances creation.</summary>
    [SimpleJob(launchCount: 1, warmupCount: 1, iterationCount: 1)]
    public class InstanceCreationBenchmark
    {
        private readonly RunnableProject project = SetupForCreateInstances();

        /// <summary>Setup a <see cref="RunnableProject"/> for benchamrk. Creates a class containing one integer field.</summary>
        private static RunnableProject SetupForCreateInstances()
        {
            WipProject theProject = Project.CreateProject(BenchmarkHelpers.ProjectName);
            _ = theProject.AddClass(BenchmarkHelpers.ClassName);
            theProject.AddField(BenchmarkHelpers.ClassName, new FieldDefinition<int>(BenchmarkHelpers.FieldName, 10));
            return ProjectBuilder.Build(theProject);
        }

        [Params(100, 200)]
        public int InstancesCount { get; set; }

        [Benchmark]
        public void CreateInstances()
        {
            project.Instances.Clear();
            for (int index = 0; index < InstancesCount; index++)
            {
                project.CreateInstance(BenchmarkHelpers.ClassName, string.Format($"{BenchmarkHelpers.InstanceName}_{index}"));
            }
        }
    }
}