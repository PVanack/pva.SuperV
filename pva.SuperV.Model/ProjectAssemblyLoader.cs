using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace pva.SuperV.Model
{
    public class ProjectAssemblyLoader : AssemblyLoadContext
    {
        public ProjectAssemblyLoader() : base(isCollectible: true)
        {
        }

        [ExcludeFromCodeCoverage]
        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}