using pva.SuperV.Model.Exceptions;
using System.Text;

namespace pva.SuperV.Model
{
    public class WipProject : Project
    {
        public WipProject()
        {
        }

        public WipProject(RunnableProject runnableProject)
        {
            this.Name = runnableProject.Name;
            this.Classes = new(runnableProject.Classes.Count);
            foreach (var item in runnableProject.Classes)
            {
                this.Classes.Add(item.Key, item.Value.Clone());
            }
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
            Classes.Remove(className);
        }

        public string GetCode()
        {
            StringBuilder codeBuilder = new();
            codeBuilder.AppendLine($"using {this.GetType().Namespace};");
            codeBuilder.AppendLine($"namespace {Name} {{");
            foreach (var item in Classes)
            {
                codeBuilder.AppendLine(item.Value.GetCode());
            }
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        public RunnableProject CloneAsRunnable()
        {
            return new RunnableProject(this);
        }
    }
}
