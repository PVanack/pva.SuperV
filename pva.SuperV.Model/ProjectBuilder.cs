using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using pva.Helpers;
using pva.SuperV.Builder.Exceptions;
using pva.SuperV.Model;
using System.Reflection;
using System.Text;

namespace pva.SuperV.Model
{
    public static class ProjectBuilder
    {
        public static RunnableProject Build(WipProject project)
        {
            string projectAssemblyFileName = project.GetAssemblyFileName();
            String projectCode = project.GetCode();
            var compilation = CreateCompilation(CSharpSyntaxTree.ParseText(projectCode), project.Name);
            var compilationResult = compilation.Emit(projectAssemblyFileName);
            if (!compilationResult.Success)
            {
                StringBuilder diagnostics = new();
                compilationResult.Diagnostics
                    .ForEach(diagnostic => diagnostics.AppendLine(diagnostic.ToString()));
                throw new ProjectBuildException(project, diagnostics.ToString());
            }
            return project.CloneAsRunnable();
        }

        private static CSharpCompilation CreateCompilation(SyntaxTree tree, string name)
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            List<MetadataReference> refs =
            [
                /* 
                * Adding some necessary .NET assemblies
                * These assemblies couldn't be loaded correctly via the same construction as above,
                * in specific the System.Runtime.
                */
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")),
                // Basic types assembly
                MetadataReference.CreateFromFile(typeof(string).Assembly.Location),
                // SuperV Project assembly
                MetadataReference.CreateFromFile(typeof(Project).Assembly.Location),
            ];

            return CSharpCompilation
                .Create(name, options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(refs)
                .AddSyntaxTrees(tree);
        }
    }
}
