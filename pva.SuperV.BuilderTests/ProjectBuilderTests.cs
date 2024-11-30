using FluentAssertions;
using pva.SuperV.Builder;
using pva.SuperV.Model;
using System.ComponentModel;

namespace pva.SuperV.BuilderTests
{
    public class ProjectBuilderTests
    {
        private const string PROJECT_NAME = "TestProject";
        private const string CLASS_NAME = "TestClass";
        private const string INSTANCE_NAME = "Instance";

        [Fact]
        public void GivenProjectWithClassAndField_WhenGettingCode_ThenGeneratedCodeIsAsExpected()
        {
            // GIVEN
            Project project = Project.CreateProject(PROJECT_NAME);
            Class clazz = project.AddClass(CLASS_NAME);
            clazz.AddField(new Field<int>("IntField", 10));

            // WHEN
            ProjectBuilder.Build(project);
            var instance = ProjectBuilder.CreateClassInstance(project, CLASS_NAME, INSTANCE_NAME);

            // THEN
            Assert.True(instance != null);
            Assert.True(instance.IntField == 10);
        }
    }
}