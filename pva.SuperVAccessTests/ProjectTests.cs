using FluentAssertions;
using pva.SuperVAccess;

namespace pva.SuperVAccessTests
{
    public class ProjectTests
    {
        private const string PROJECT_NAME = "TestProject";

        [Fact]
        public void Test1()
        {
            Project project = new() { Name = PROJECT_NAME };

            project.Name.Should().Be(PROJECT_NAME);
        }
    }
}