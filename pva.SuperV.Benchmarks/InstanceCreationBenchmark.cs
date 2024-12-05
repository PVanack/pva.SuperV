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
            WipProject theProject = Project.CreateProject(BenchmarkHelpers.PROJECT_NAME);
            Class clazz = theProject.AddClass(BenchmarkHelpers.CLASS_NAME);
            clazz.AddField(new FieldDefinition<int>(BenchmarkHelpers.FIELD_NAME, 10));
            return ProjectBuilder.Build(theProject);
        }

        [Params(100, 200)]
        public int InstanceesCount { get; set; }

        [Benchmark]
        public void CreateInstances()
        {
            for (int index = 0; index < InstanceesCount; index++)
            {
                project.CreateClassInstance(BenchmarkHelpers.CLASS_NAME, String.Format($"{BenchmarkHelpers.INSTANCE_NAME}-{index}"));
            }
        }
    }
}
