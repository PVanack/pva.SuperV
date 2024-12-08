using BenchmarkDotNet.Attributes;
using pva.SuperV.Builder;
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
            Class clazz = theProject.AddClass(BenchmarkHelpers.ClassName);
            clazz.AddField(new FieldDefinition<int>(BenchmarkHelpers.FieldName, 10));
            return ProjectBuilder.Build(theProject);
        }

        [Params(100, 200)]
        public int InstanceesCount { get; set; }

        [Benchmark]
        public void CreateInstances()
        {
            for (int index = 0; index < InstanceesCount; index++)
            {
                project.CreateClassInstance(BenchmarkHelpers.ClassName, String.Format($"{BenchmarkHelpers.InstanceName}-{index}"));
            }
        }
    }
}
