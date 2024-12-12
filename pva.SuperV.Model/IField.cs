namespace pva.SuperV.Model
{
    public interface IField<T>: IDisposable
    {
        Type Type { get; }
        public T Value { get; set; }
    }
}