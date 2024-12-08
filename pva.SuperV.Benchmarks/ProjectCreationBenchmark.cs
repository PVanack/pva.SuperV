using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using pva.SuperV.Builder;
using pva.SuperV.Model;

namespace pva.SuperV.Benchmarks
{
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
                Class clazz = theProject.AddClass(String.Format($"{BenchmarkHelpers.ClassName}_{classIndex}"));
                for (int fieldIndex = 0; fieldIndex < FieldsCount; fieldIndex++)
                {
                    clazz.AddField(new FieldDefinition<int>(String.Format($"{BenchmarkHelpers.FieldName}_{fieldIndex}"), fieldIndex));
                }
            }
            ProjectBuilder.Build(theProject);
        }
    }

    [SimpleJob(RunStrategy.Throughput)]
    public class InstanceCreation
    {
        private readonly RunnableProject runnableProject = SetupForCreateInstances();

        public static RunnableProject SetupForCreateInstances()
        {
            WipProject wipProject = Project.CreateProject(BenchmarkHelpers.ProjectName);
            Class clazz = wipProject.AddClass(BenchmarkHelpers.ClassName);
            clazz.AddField(new FieldDefinition<int>(BenchmarkHelpers.FieldName, 10));
            return ProjectBuilder.Build(wipProject);
        }

        [Params(100, 200)]
        public int InstancesCount { get; set; }

        [Benchmark]
        public void CreateInstances()
        {
            for (int index = 0; index < InstancesCount; index++)
            {
                runnableProject.CreateClassInstance(BenchmarkHelpers.ClassName, String.Format($"{BenchmarkHelpers.InstanceName}-{index}"));
            }
        }
    }
}
