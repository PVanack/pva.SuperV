using pva.Helpers;
using pva.SuperV.Model.Exceptions;

namespace pva.SuperV.Model
{
    public class RunnableProject : Project, IDisposable
    {
        private ProjectAssemblyLoader _projectAssemblyLoader;
        public Dictionary<String, dynamic> Instances { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        public RunnableProject(WipProject wipProject)
        {
            this.Name = wipProject.Name;
            this.Classes = wipProject.Classes;
            this._projectAssemblyLoader = new();
            this._projectAssemblyLoader.LoadFromAssemblyPath(GetAssemblyFileName());
        }

        public dynamic? CreateInstance(string className, string instanceName)
        {
            Class clazz = GetClass(className);
            string classFullName = $"{Name}.{clazz.Name}";
            dynamic? dynamicInstance = Activator.CreateInstanceFrom(GetAssemblyFileName(), classFullName)
                ?.Unwrap();
            if (dynamicInstance != null)
            {
                dynamicInstance.Name = instanceName;
                dynamicInstance.Class = clazz;
                Instances.Add(instanceName, dynamicInstance);
            }
            return dynamicInstance;
        }

        public void RemoveInstance(string instanceName)
        {
            Instances.Remove(instanceName);
        }

        public IInstance GetInstance(string instanceName)
        {
            if (Instances.TryGetValue(instanceName, out var instance))
            {
                return instance;
            }
            throw new UnknownInstanceException(instanceName);
        }

        public void Unload()
        {
            Classes.Clear();
            Instances.Clear();
            _projectAssemblyLoader.Unload();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            while (_projectAssemblyLoader.Assemblies != null && _projectAssemblyLoader.Assemblies.Any())
            {
                Thread.Sleep(100);
            }
            _projectAssemblyLoader = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Unload();
        }
    }
}
