namespace pva.SuperVAccess
{
    public class Field<T>(String name, T? value)
    {
        public String Name { get; set; } = name;
        public T? Value { get; set; } = value ?? default;
    }
}
