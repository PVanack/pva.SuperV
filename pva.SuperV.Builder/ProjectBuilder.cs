using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using pva.SuperV.Builder.Exceptions;
using pva.SuperV.Model;
using System.Text;

namespace pva.SuperV.Builder
{
    public static class ProjectBuilder
    {
        public static Project Build(Project project)
        {
            string projectAssemblyFileName = project.GetAssemblyFileName();
            String projectCode = project.GetCode();
            var compilation = CreateCompilation(CSharpSyntaxTree.ParseText(projectCode), project.Name);
            var compilationResult = compilation.Emit(projectAssemblyFileName);
            if (!compilationResult.Success)
            {
                StringBuilder diagnostics = new();
                foreach (var item in compilationResult.Diagnostics)
                {
                    diagnostics.AppendLine(item.ToString());
                }
                throw new ProjectBuildException(project, diagnostics.ToString());
            }
            return project;
        }

        private static CSharpCompilation CreateCompilation(SyntaxTree tree, string name) =>
            CSharpCompilation
                .Create(name, options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
                .AddSyntaxTrees(tree);
    }
}
