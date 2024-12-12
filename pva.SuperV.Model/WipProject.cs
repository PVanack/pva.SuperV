using pva.Helpers;
using pva.SuperV.Model.Exceptions;
using System.Text;

namespace pva.SuperV.Model
{
    public class WipProject : Project
    {
        private Dictionary<String, dynamic> ToLoadInstances { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        public WipProject(string projectName)
        {
            Name = projectName;
        }

        public WipProject(RunnableProject runnableProject)
        {
            this.Name = runnableProject.Name;
            this.Classes = new(runnableProject.Classes.Count);
            runnableProject.Classes
                .ForEach((k, v) => this.Classes.Add(k, v.Clone()));
            this.ToLoadInstances = new Dictionary<String, dynamic>(runnableProject.Instances, StringComparer.OrdinalIgnoreCase);
        }

        public Class AddClass(String className)

        {
            if (Classes.ContainsKey(className))
            {
                throw new ClassAlreadyExistException(className);
            }

            Class clazz = new(className);
            Classes.Add(className, clazz);
            return clazz;
        }

        public void RemoveClass(String className)
        {
            if (Classes.TryGetValue(className, out var clazz))
            {
                Classes.Remove(className);
                ToLoadInstances.Values
                    .Where(instance => instance.Class.Name.Equals(clazz.Name))
                    .ToList()
                    .ForEach(instanceName => ToLoadInstances.Remove(instanceName));
            }
        }

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"using {this.GetType().Namespace};");
            codeBuilder.AppendLine($"namespace {Name} {{");
            Classes
                .ForEach((_, v) => codeBuilder.AppendLine(v.GetCode()));
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        public RunnableProject CloneAsRunnable()
        {
            return new RunnableProject(this);
        }
    }
}
