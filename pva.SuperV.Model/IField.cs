namespace pva.SuperV.Model
{
    public interface IField<T>
    {
        Type Type { get; }
        public T Value { get; set; }
    }
}