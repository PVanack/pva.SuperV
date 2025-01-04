using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using pva.Helpers.Extensions;
using pva.SuperV.Engine.Exceptions;
using System.Text;

namespace pva.SuperV.Engine
{
    /// <summary>
    /// Class for building a <see cref="WipProject"/> and convert it to a <see cref="RunnableProject"/> by generating an assembly and creating instances.
    /// </summary>
    public static class ProjectBuilder
    {
        /// <summary>
        /// Builds the specified <see cref="WipProject"/>.
        /// </summary>
        /// <param name="project">The WIP project.</param>
        /// <returns>a <see cref="RunnableProject"/></returns>
        /// <exception cref="pva.SuperV.Engine.Exceptions.ProjectBuildException"></exception>
        public static RunnableProject Build(WipProject project)
        {
            string projectAssemblyFileName = project.GetAssemblyFileName();
            string projectCode = project.GetCode();
            var compilation = CreateCompilation(CSharpSyntaxTree.ParseText(projectCode), $"{project.Name}-V{project.Version}");
            using (MemoryStream dllStream = new())
            using (MemoryStream pdbStream = new())
            using (Stream win32resStream = compilation.CreateDefaultWin32Resources(
                versionResource: true, // Important!
                noManifest: false,
                manifestContents: null,
                iconInIcoFormat: null))
            {
                var compilationResult = compilation.Emit(
                    peStream: dllStream,
                    pdbStream: pdbStream,
                    win32Resources: win32resStream);

                if (!compilationResult.Success)
                {
                    StringBuilder diagnostics = new();
                    compilationResult.Diagnostics
                        .ForEach(diagnostic => diagnostics.AppendLine(diagnostic.ToString()));
                    throw new ProjectBuildException(project, diagnostics.ToString());
                }
                File.WriteAllBytes(projectAssemblyFileName, dllStream.ToArray());
            }
            return project.CloneAsRunnable();
        }

        /// <summary>
        /// Creates a <see cref="CSharpCompilation"/> to generate an assembly for the project.
        /// </summary>
        /// <param name="tree">The tree.</param>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="CSharpCompilation"/></returns>
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
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.Runtime.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.Collections.dll")),
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