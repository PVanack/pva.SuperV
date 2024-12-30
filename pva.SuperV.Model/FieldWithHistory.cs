namespace pva.SuperV.Model
{
    internal class FieldWithHistory<T>(T value) : Field<T>(value)
    {
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