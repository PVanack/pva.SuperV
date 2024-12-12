namespace pva.SuperV.Model
{
    internal class FieldWithHistory<T> : Field<T>
    {
        public FieldWithHistory(T value) : base(value)
        {
        }

        public override T Value
        {
            get => base.Value;
            set
            {
                base.Value = value;
                Historize();
            }
        }

        private void Historize()
        {

        }
    }
}
