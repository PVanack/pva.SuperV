using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace pva.SuperV.Model
{
    /// <summary>
    /// Project generated assembly loader. Creates an <see cref="AssemblyLoadContext"/> so that the generated assembly can be unloaded when the project is <see cref="Project.Unload"/>
    /// </summary>
    /// <seealso cref="System.Runtime.Loader.AssemblyLoadContext" />
    public class ProjectAssemblyLoader : AssemblyLoadContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectAssemblyLoader"/> class.
        /// </summary>
        public ProjectAssemblyLoader() : base(isCollectible: true)
        {
        }

        /// <summary>
        /// When overridden in a derived class, allows an assembly to be resolved based on its <see cref="T:System.Reflection.AssemblyName" />.
        /// </summary>
        /// <param name="assemblyName">The object that describes the assembly to be resolved.</param>
        /// <returns>
        /// The resolved assembly, or <see langword="null" />.
        /// </returns>
        [ExcludeFromCodeCoverage]
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}