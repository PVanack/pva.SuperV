namespace pva.SuperV.Model
{
    public class Field<T>(String name, T? value)
    {
        public String Name { get; set; } = name;
        public T? Value { get; set; } = value ?? default;
    }
}
