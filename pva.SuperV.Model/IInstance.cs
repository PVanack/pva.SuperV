namespace pva.SuperV.Model
{
    public interface IInstance : IDisposable
    {
        public String Name { get; set; }
        public Class Class { get; set; }
        //public IField<T> GetField<T>(String name);
    }
}
