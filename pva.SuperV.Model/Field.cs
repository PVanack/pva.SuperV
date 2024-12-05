using System.Text;

namespace pva.SuperV.Model
{
    public class Field<T>(T? value): IField
    {
        public T? Value { get; set; } = value ?? default;

        IField Clone()
        {
            return new Field<T>(this.Value);
        }
    }
}
