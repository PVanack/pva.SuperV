using pva.Helpers.Extensions;
using pva.SuperV.Model.Exceptions;
using System.Text.Json.Serialization;

namespace pva.SuperV.Model
{
    public class RunnableProject : Project
    {
        [JsonIgnore]
        private ProjectAssemblyLoader? _projectAssemblyLoader;

        [JsonIgnore]
        public Dictionary<string, dynamic> Instances { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        public RunnableProject()
        {
        }

        public RunnableProject(WipProject wipProject)
        {
            this.Name = wipProject.Name;
            this.Version = wipProject.Version;
            this.Classes = new(wipProject.Classes);
            this._projectAssemblyLoader = new();
            this._projectAssemblyLoader.LoadFromAssemblyPath(GetAssemblyFileName());
            RecreateInstances(wipProject);
        }

        public dynamic? CreateInstance(string className, string instanceName)
        {
            if (Instances.ContainsKey(instanceName))
            {
                throw new InstanceAlreadyExistException(instanceName);
            }
            Class clazz = GetClass(className);
            string classFullName = $"{Name}.V{Version}.{clazz.Name}";
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

        public Instance GetInstance(string instanceName)
        {
            if (Instances.TryGetValue(instanceName, out var instance))
            {
                return instance;
            }
            throw new UnknownInstanceException(instanceName);
        }

        private void RecreateInstances(WipProject wipProject)
        {
            wipProject.ToLoadInstances
                .ForEach((k, v) =>
                {
                    string instanceName = k;
                    Instance oldInstance = v;
                    Instance newInstance = CreateInstance(oldInstance.Class.Name, instanceName);
                    Dictionary<string, IField> newFields = new(newInstance.Fields.Count);
                    newInstance.Fields
                        .ForEach((k, v) =>
                        {
                            string fieldName = k;
                            if (oldInstance.Fields.TryGetValue(fieldName, out IField? oldField))
                            {
                                newFields.Add(fieldName, oldField);
                            }
                            else
                            {
                                newFields.Add(fieldName, v);
                            }
                        });
                    newInstance.Fields = newFields;
                });
        }

        public override void Unload()
        {
            Instances.Values.ForEach(instance =>
                instance.Dispose());
            Instances.Clear();
            _projectAssemblyLoader?.Unload();
            _projectAssemblyLoader = null;
            base.Unload();
        }
    }
}