using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace pva.SuperV.Blazor.Components.Pages
{
    public partial class FieldValue : ComponentBase
    {
        [BindProperty]
        [Parameter]
#pragma warning disable BL0007 // Component parameters should be auto properties
        public object? Value
#pragma warning restore BL0007 // Component parameters should be auto properties
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
        string? StringValue { get; set; } = default;
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
            bool valueChanged = value?.Equals(_value) == false;
            _value = value;
            if (value != null)
            {
                switch (value)
                {
                    case bool boolValue:
                        BoolValue = boolValue;
                        break;
                    case DateTime dateTimeValue:
                        DateTimeValue = dateTimeValue;
                        break;
                    case double doubleValue:
                        DoubleValue = doubleValue;
                        break;
                    case float floatValue:
                        FloatValue = floatValue;
                        break;
                    case int intValue:
                        IntValue = intValue;
                        break;
                    case long longValue:
                        LongValue = longValue;
                        break;
                    case short shortValue:
                        ShortValue = shortValue;
                        break;
                    case string stringValue:
                        StringValue = stringValue;
                        break;
                    case TimeSpan timespanValue:
                        TimeSpanValue = timespanValue;
                        break;
                    case uint uintValue:
                        UintValue = uintValue;
                        break;
                    case ulong ulongValue:
                        UlongValue = ulongValue;
                        break;
                    case ushort ushortValue:
                        UshortValue = ushortValue;
                        break;
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