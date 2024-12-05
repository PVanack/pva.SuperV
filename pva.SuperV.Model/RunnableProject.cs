namespace pva.SuperV.Model
{
    public class RunnableProject : Project
    {
        public RunnableProject(WipProject wipProject)
        {
            this.Name = wipProject.Name;
            this.Classes = wipProject.Classes;
        }

        public dynamic? CreateClassInstance(string className, string instanceName)
        {
            Class clazz = GetClass(className);
            string classFullName = $"{Name}.{clazz.Name}";
            dynamic? instance = Activator.CreateInstanceFrom(GetAssemblyFileName(), classFullName)
                ?.Unwrap();
            instance.Name = instanceName;
            return instance;
        }
    }
}
