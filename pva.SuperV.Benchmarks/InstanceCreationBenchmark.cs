using BenchmarkDotNet.Attributes;
using pva.SuperV.Model;

namespace pva.SuperV.Benchmarks
{
    [SimpleJob(launchCount: 1, warmupCount: 1, iterationCount: 1)]
    public class InstanceCreationBenchmark
    {
        private readonly RunnableProject project = SetupForCreateInstances();

        public static RunnableProject SetupForCreateInstances()
        {
            WipProject theProject = Project.CreateProject(BenchmarkHelpers.ProjectName);
            _ = theProject.AddClass(BenchmarkHelpers.ClassName);
            theProject.AddField(BenchmarkHelpers.ClassName, new FieldDefinition<int>(BenchmarkHelpers.FieldName, 10));
            return ProjectBuilder.Build(theProject);
        }

        [Params(100, 200)]
        public int InstanceesCount { get; set; }

        [Benchmark]
        public void CreateInstances()
        {
            project.Instances.Clear();
            for (int index = 0; index < InstanceesCount; index++)
            {
                project.CreateInstance(BenchmarkHelpers.ClassName, string.Format($"{BenchmarkHelpers.InstanceName}_{index}"));
            }
        }
    }
}