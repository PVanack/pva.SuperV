using NSubstitute;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using Shouldly;
using Xunit;

namespace pva.SuperV.EngineTests
{
    public class AlarmStateProcessingTests
    {
        private const string ProcessingName = "TestClass_AlmProcessing";
        private const string ValueFieldName = "Value";
        private const string HighHighLimitFieldName = "HighHigh";
        private const string HighLimitFieldName = "High";
        private const string LowLimitFieldName = "Low";
        private const string LowLowLimitFieldName = "LowLow";
        private const string DeadbandFieldName = "Deadband";
        private const string AlarmStateFieldName = "AlarmState";
        private const string AckStateFieldName = "AckState";

        private readonly Class clazz;
        private readonly FieldDefinition<int> alarmStateFieldDefinition = new(AlarmStateFieldName, 0);
        private readonly FieldDefinition<int> ackStateFieldDefinition = new(AckStateFieldName, 0);
        private readonly AlarmStateProcessing<double> alarmStateProcessing;

        private readonly IInstance instance = Substitute.For<IInstance>();

        public AlarmStateProcessingTests()
        {
            clazz = new("TestClass");
            clazz.FieldDefinitions.Add(ValueFieldName, new FieldDefinition<double>(ValueFieldName, 50));
            clazz.FieldDefinitions.Add(HighHighLimitFieldName, new FieldDefinition<double>(HighHighLimitFieldName, 100));
            clazz.FieldDefinitions.Add(HighLimitFieldName, new FieldDefinition<double>(HighLimitFieldName, 75));
            clazz.FieldDefinitions.Add(LowLimitFieldName, new FieldDefinition<double>(LowLimitFieldName, 25));
            clazz.FieldDefinitions.Add(LowLowLimitFieldName, new FieldDefinition<double>(LowLowLimitFieldName, 0));
            clazz.FieldDefinitions.Add(DeadbandFieldName, new FieldDefinition<double>(DeadbandFieldName, 1));
            clazz.FieldDefinitions.Add(AlarmStateFieldName, alarmStateFieldDefinition);
            clazz.FieldDefinitions.Add(AckStateFieldName, ackStateFieldDefinition);
            alarmStateProcessing = new(ProcessingName, clazz, ValueFieldName,
                HighHighLimitFieldName, HighLimitFieldName, LowLimitFieldName, LowLowLimitFieldName,
                DeadbandFieldName, AlarmStateFieldName, AckStateFieldName);
            clazz.AddFieldChangePostProcessing(ValueFieldName, alarmStateProcessing);


            instance.GetField<double>(HighHighLimitFieldName).Returns(new Field<double>(100.0));
            instance.GetField<double>(HighLimitFieldName).Returns(new Field<double>(75.0));
            instance.GetField<double>(LowLimitFieldName).Returns(new Field<double>(25.0));
            instance.GetField<double>(LowLowLimitFieldName).Returns(new Field<double>(0.0));
            instance.GetField<double>(DeadbandFieldName).Returns(new Field<double>(0.0));
        }

        [Fact]
        public void GivenAlarmStateOkAndUnack_WhenChangingValueAboveHighHighLimit_ThenAlarmStateIsHighHighAndUnack()
        {
            // GIVEN
            Field<int> alarmStateField = new(0)
            {
                FieldDefinition = alarmStateFieldDefinition
            };
            Field<int> ackStateField = new(0)
            {
                FieldDefinition = ackStateFieldDefinition
            };
            instance.GetField<int>(AlarmStateFieldName).Returns(alarmStateField);
            instance.GetField<int>(AckStateFieldName).Returns(ackStateField);

            // WHEN
            Field<double> valueField = new(110.0);
            alarmStateProcessing.ProcessValue(instance, valueField, true, 50.0, valueField.Value);

            // THEN
            alarmStateField.Value.ShouldBe(2);
            ackStateField.Value.ShouldBe(1);
        }

        [Fact]
        public void GivenAlarmStateOkAndUnack_WhenChangingValueAboveHighLimit_ThenAlarmStateIsHighAndUnack()
        {
            // GIVEN
            Field<int> alarmStateField = new(0)
            {
                FieldDefinition = alarmStateFieldDefinition
            };
            Field<int> ackStateField = new(0)
            {
                FieldDefinition = ackStateFieldDefinition
            };
            instance.GetField<int>(AlarmStateFieldName).Returns(alarmStateField);
            instance.GetField<int>(AckStateFieldName).Returns(ackStateField);

            // WHEN
            Field<double> valueField = new(80.0);
            alarmStateProcessing.ProcessValue(instance, valueField, true, 50.0, valueField.Value);

            // THEN
            alarmStateField.Value.ShouldBe(1);
            ackStateField.Value.ShouldBe(1);
        }

        [Fact]
        public void GivenAlarmStateOkAndUnack_WhenChangingValueBelowLowLimit_ThenAlarmStateIsLowAndUnack()
        {
            // GIVEN
            Field<int> alarmStateField = new(0)
            {
                FieldDefinition = alarmStateFieldDefinition
            };
            Field<int> ackStateField = new(0)
            {
                FieldDefinition = ackStateFieldDefinition
            };
            instance.GetField<int>(AlarmStateFieldName).Returns(alarmStateField);
            instance.GetField<int>(AckStateFieldName).Returns(ackStateField);

            // WHEN
            Field<double> valueField = new(20.0);
            alarmStateProcessing.ProcessValue(instance, valueField, true, 50.0, valueField.Value);

            // THEN
            alarmStateField.Value.ShouldBe(-1);
            ackStateField.Value.ShouldBe(1);
        }

        [Fact]
        public void GivenAlarmStateOkAndUnack_WhenChangingValueBelowLowLowLimit_ThenAlarmStateIsLowLowAndUnack()
        {
            // GIVEN
            Field<int> alarmStateField = new(0)
            {
                FieldDefinition = alarmStateFieldDefinition
            };
            Field<int> ackStateField = new(0)
            {
                FieldDefinition = ackStateFieldDefinition
            };
            instance.GetField<int>(AlarmStateFieldName).Returns(alarmStateField);
            instance.GetField<int>(AckStateFieldName).Returns(ackStateField);

            // WHEN
            Field<double> valueField = new(-1.0);
            alarmStateProcessing.ProcessValue(instance, valueField, true, 50.0, valueField.Value);

            // THEN
            alarmStateField.Value.ShouldBe(-2);
            ackStateField.Value.ShouldBe(1);
        }

        [Fact]
        public void GivenAlarmStateNotOkkAndUnack_WhenChangingValueBetweenHighAndLowLimits_ThenAlarmStateIsOkwAndAck()
        {
            // GIVEN
            Field<int> alarmStateField = new(0)
            {
                FieldDefinition = alarmStateFieldDefinition
            };
            Field<int> ackStateField = new(0)
            {
                FieldDefinition = ackStateFieldDefinition
            };
            instance.GetField<int>(AlarmStateFieldName).Returns(alarmStateField);
            instance.GetField<int>(AckStateFieldName).Returns(ackStateField);

            // WHEN
            Field<double> valueField = new(52);
            alarmStateProcessing.ProcessValue(instance, valueField, true, 50.0, valueField.Value);

            // THEN
            alarmStateField.Value.ShouldBe(0);
            ackStateField.Value.ShouldBe(0);
        }

        [Theory]
        [InlineData(HighHighLimitFieldName)]
        [InlineData(HighLimitFieldName)]
        [InlineData(LowLimitFieldName)]
        [InlineData(LowLowLimitFieldName)]
        [InlineData(DeadbandFieldName)]
        [InlineData(AlarmStateFieldName)]
        [InlineData(AckStateFieldName)]
        public void GivenClassWithAlarmState_WhenRemovingFieldUsedInProcessing_ThenEntityInUseExceptionIsThrown(string usedField)
        {
            // WHEN/THEN
            Assert.Throws<EntityInUseException>(() => clazz.RemoveField(usedField));
        }


    }
}
