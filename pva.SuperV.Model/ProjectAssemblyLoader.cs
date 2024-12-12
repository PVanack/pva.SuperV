using System.Reflection;
using System.Runtime.Loader;

namespace pva.SuperV.Model
{
    public class ProjectAssemblyLoader : AssemblyLoadContext
    {
        public ProjectAssemblyLoader() : base(isCollectible: true)
        {
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}
