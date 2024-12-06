using System.Text;

namespace pva.SuperV.Model
{
    public class Field<T>(T value): IField<T>
    {
        public Type Type => typeof(T);
        public T Value { get; set; } = value;
    }
}
