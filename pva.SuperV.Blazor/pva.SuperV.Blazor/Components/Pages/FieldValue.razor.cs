using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class FieldValue : ComponentBase
    {
        [BindProperty]
        [Parameter]
        public object? Value
        {
            get => GetValue();
            set => SetValue(value);
        }
        [Parameter]
        public EventCallback<object?> ValueChanged { get; set; }
        [Parameter]
        public string ValueLabel { get; set; } = default!;

        private bool initCalled = false;
        object? _value;
        bool? BoolValue { get; set; } = default!;
        DateTime? DateTimeValue { get; set; } = default!;
        double? DoubleValue { get; set; } = default!;
        float? FloatValue { get; set; } = default!;
        int? IntValue { get; set; } = default!;
        long? LongValue { get; set; } = default!;
        short? ShortValue { get; set; } = default!;
        string? StringValue { get; set; } = default!;
        TimeSpan? TimeSpanValue { get; set; } = default!;
        uint? UintValue { get; set; } = default!;
        ulong? UlongValue { get; set; } = default!;
        ushort? UshortValue { get; set; } = default!;

        protected override void OnInitialized()
        {
            SetValue(_value);
            initCalled = true;
            base.OnInitialized();
        }

        private void SetValue(object? value)
        {
            bool valueChanged = value != null && !value.Equals(_value);
            _value = value;
            if (value != null)
            {
                if (value is bool boolValue)
                {
                    BoolValue = boolValue;
                }
                else if (value is DateTime dateTimeValue)
                {
                    DateTimeValue = dateTimeValue;
                }
                else if (value is double doubleValue)
                {
                    DoubleValue = doubleValue;
                }
                else if (value is float floatValue)
                {
                    FloatValue = floatValue;
                }
                else if (value is int intValue)
                {
                    IntValue = intValue;
                }
                else if (value is long longValue)
                {
                    LongValue = longValue;
                }
                else if (value is short shortValue)
                {
                    ShortValue = shortValue;
                }
                else if (value is string stringValue)
                {
                    StringValue = stringValue;
                }
                else if (value is TimeSpan timespanValue)
                {
                    TimeSpanValue = timespanValue;
                }
                else if (value is uint uintValue)
                {
                    UintValue = uintValue;
                }
                else if (value is ulong ulongValue)
                {
                    UlongValue = ulongValue;
                }
                else if (value is ushort ushortValue)
                {
                    UshortValue = ushortValue;
                }
            }
            if (initCalled && valueChanged)
            {
                ValueChanged.InvokeAsync(value);
            }

        }

        private object? GetValue()
        {
            _value = _value switch
            {
                bool => BoolValue,
                DateTime => DateTimeValue,
                double => DoubleValue,
                float => FloatValue,
                int => IntValue,
                long => LongValue,
                short => ShortValue,
                string => StringValue,
                TimeSpan => TimeSpanValue,
                uint => UintValue,
                ulong => UlongValue,
                ushort => UshortValue,
                _ => StringValue
            };
            return _value;
        }
    }
}