﻿using pva.Helpers;
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
            runnableProject.Classes
                .ForEach((k, v) => this.Classes.Add(k, v.Clone()));
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
            Classes
                .ForEach((k, v) => codeBuilder.AppendLine(v.GetCode()));
            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        public RunnableProject CloneAsRunnable()
        {
            return new RunnableProject(this);
        }
    }
}
