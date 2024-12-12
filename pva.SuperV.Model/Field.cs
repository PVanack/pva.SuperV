namespace pva.SuperV.Model
{
    public class Field<T>(T value) : IField
    {
        public Type Type => typeof(T);
        public virtual T Value { get; set; } = value;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
