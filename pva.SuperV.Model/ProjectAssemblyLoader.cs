using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

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
