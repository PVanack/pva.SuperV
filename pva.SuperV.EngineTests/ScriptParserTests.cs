using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.Processing;
using Shouldly;

namespace pva.SuperV.EngineTests
{
    public class ScriptParserTests
    {
        #region ParseLine Tests

        [Fact]
        public void GivenSimpleScript_WhenParsingLine_ThenSingleLineIsReturned()
        {
            // GIVEN
            const string script = "Line1";

            // WHEN
            List<string> result = ScriptParser.ParseLine(script);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].ShouldBe("Line1");
        }

        [Fact]
        public void GivenMultiLineScript_WhenParsingLine_ThenAllLinesAreReturned()
        {
            // GIVEN
            const string script = "Line1\nLine2\nLine3";

            // WHEN
            List<string> result = ScriptParser.ParseLine(script);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result[0].ShouldBe("Line1");
            result[1].ShouldBe("Line2");
            result[2].ShouldBe("Line3");
        }

        [Fact]
        public void GivenScriptWithEmptyLines_WhenParsingLine_ThenEmptyLinesAreSkipped()
        {
            // GIVEN
            const string script = "Line1\n\nLine2\n\n\nLine3";

            // WHEN
            List<string> result = ScriptParser.ParseLine(script);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result[0].ShouldBe("Line1");
            result[1].ShouldBe("Line2");
            result[2].ShouldBe("Line3");
        }

        [Fact]
        public void GivenScriptWithWhitespaceLines_WhenParsingLine_ThenWhitespaceOnlyLinesAreSkipped()
        {
            // GIVEN
            const string script = "Line1\n   \nLine2\n\t\t\nLine3";

            // WHEN
            List<string> result = ScriptParser.ParseLine(script);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result[0].ShouldBe("Line1");
            result[1].ShouldBe("Line2");
            result[2].ShouldBe("Line3");
        }

        [Fact]
        public void GivenScriptWithLeadingAndTrailingWhitespace_WhenParsingLine_ThenWhitespaceIsTrimmed()
        {
            // GIVEN
            const string script = "  Line1  \n  Line2  \n  Line3  ";

            // WHEN
            List<string> result = ScriptParser.ParseLine(script);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result[0].ShouldBe("Line1");
            result[1].ShouldBe("Line2");
            result[2].ShouldBe("Line3");
        }

        [Fact]
        public void GivenEmptyScript_WhenParsingLine_ThenEmptyListIsReturned()
        {
            // GIVEN
            const string script = "";

            // WHEN
            List<string> result = ScriptParser.ParseLine(script);

            // THEN
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public void GivenScriptWithOnlyEmptyLines_WhenParsingLine_ThenEmptyListIsReturned()
        {
            // GIVEN
            const string script = "\n\n\n";

            // WHEN
            List<string> result = ScriptParser.ParseLine(script);

            // THEN
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public void GivenScriptWithCRLF_WhenParsingLine_ThenAllLinesAreReturned()
        {
            // GIVEN
            const string script = "Line1\r\nLine2\r\nLine3";

            // WHEN
            List<string> result = ScriptParser.ParseLine(script);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result[0].ShouldBe("Line1");
            result[1].ShouldBe("Line2");
            result[2].ShouldBe("Line3");
        }

        #endregion

        #region ParseFieldReferences Tests

        [Fact]
        public void GivenLineWithNoFieldReferences_WhenParsingFieldReferences_ThenEmptyListIsReturned()
        {
            // GIVEN
            List<string> lines = ["Line1", "Line2"];

            // WHEN
            List<FieldReference> result = ScriptParser.ParseFieldReferences(lines);

            // THEN
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public void GivenLineWithSingleFieldReference_WhenParsingFieldReferences_ThenReferenceIsReturned()
        {
            // GIVEN
            List<string> lines = ["{{Field1}}"];

            // WHEN
            List<FieldReference> result = ScriptParser.ParseFieldReferences(lines);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].InstanceName.ShouldBeNull();
            result[0].FieldName.ShouldBe("Field1");
        }

        [Fact]
        public void GivenLineWithFieldReferenceWithInstance_WhenParsingFieldReferences_ThenReferenceWithInstanceIsReturned()
        {
            // GIVEN
            List<string> lines = ["{{Instance1.Field1}}"];

            // WHEN
            List<FieldReference> result = ScriptParser.ParseFieldReferences(lines);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].InstanceName.ShouldBe("Instance1");
            result[0].FieldName.ShouldBe("Field1");
        }

        [Fact]
        public void GivenLineWithMultipleFieldReferences_WhenParsingFieldReferences_ThenAllReferencesAreReturned()
        {
            // GIVEN
            List<string> lines = ["{{Field1}} and {{Field2}} and {{Instance1.Field3}}"];

            // WHEN
            List<FieldReference> result = ScriptParser.ParseFieldReferences(lines);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result[0].InstanceName.ShouldBeNull();
            result[0].FieldName.ShouldBe("Field1");
            result[1].InstanceName.ShouldBeNull();
            result[1].FieldName.ShouldBe("Field2");
            result[2].InstanceName.ShouldBe("Instance1");
            result[2].FieldName.ShouldBe("Field3");
        }

        [Fact]
        public void GivenMultipleLineWithFieldReferences_WhenParsingFieldReferences_ThenAllReferencesAreReturned()
        {
            // GIVEN
            List<string> lines = ["{{Field1}}", "{{Instance1.Field2}}", "{{Field3}}"];

            // WHEN
            List<FieldReference> result = ScriptParser.ParseFieldReferences(lines);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
        }

        [Fact]
        public void GivenLineWithMissingClosingBraces_WhenParsingFieldReferences_ThenScriptSyntaxErrorExceptionIsThrown()
        {
            // GIVEN
            List<string> lines = ["{{Field1"];

            // WHEN/THEN
            ScriptSyntaxErrorException exception = Assert.Throws<ScriptSyntaxErrorException>(() => ScriptParser.ParseFieldReferences(lines));
            exception.Message.ShouldContain("Missing }}");
        }

        [Fact]
        public void GivenLineWithEmptyFieldReference_WhenParsingFieldReferences_ThenScriptSyntaxErrorExceptionIsThrown()
        {
            // GIVEN
            List<string> lines = ["{{}}"];

            // WHEN/THEN
            ScriptSyntaxErrorException exception = Assert.Throws<ScriptSyntaxErrorException>(() => ScriptParser.ParseFieldReferences(lines));
            exception.Message.ShouldContain("Empty field reference");
        }

        [Fact]
        public void GivenFieldReferenceWithInvalidIdentifier_WhenParsingFieldReferences_ThenInvalidIdentifierNameExceptionIsThrown()
        {
            // GIVEN
            List<string> lines = ["{{0InvalidField}}"];

            // WHEN/THEN
            Assert.Throws<InvalidIdentifierNameException>(() => ScriptParser.ParseFieldReferences(lines));
        }

        [Fact]
        public void GivenFieldReferenceWithInvalidInstanceName_WhenParsingFieldReferences_ThenInvalidIdentifierNameExceptionIsThrown()
        {
            // GIVEN
            List<string> lines = ["{{0InvalidInstance.Field1}}"];

            // WHEN/THEN
            Assert.Throws<InvalidIdentifierNameException>(() => ScriptParser.ParseFieldReferences(lines));
        }

        [Fact]
        public void GivenFieldReferenceWithInvalidFieldName_WhenParsingFieldReferences_ThenInvalidIdentifierNameExceptionIsThrown()
        {
            // GIVEN
            List<string> lines = ["{{Instance1.0InvalidField}}"];

            // WHEN/THEN
            Assert.Throws<InvalidIdentifierNameException>(() => ScriptParser.ParseFieldReferences(lines));
        }

        [Fact]
        public void GivenDuplicateFieldReferences_WhenParsingFieldReferences_ThenOnlyBothAreReturned()
        {
            // GIVEN
            List<string> lines = ["{{Field1}} and {{Field1}}"];

            // WHEN
            List<FieldReference> result = ScriptParser.ParseFieldReferences(lines);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result[0].FieldName.ShouldBe("Field1");
            result[1].FieldName.ShouldBe("Field1");
        }

        [Fact]
        public void GivenNestedFieldReference_WhenParsingFieldReferences_ThenOnlyValidReferencesAreFound()
        {
            // GIVEN
            List<string> lines = ["Prefix {{Field1 {{Nested}} }} Suffix"];

            // WHEN/THEN
            Assert.Throws<InvalidIdentifierNameException>(() => ScriptParser.ParseFieldReferences(lines));

            // THEN
        }

        [Fact]
        public void GivenEmptyLines_WhenParsingFieldReferences_ThenEmptyListIsReturned()
        {
            // GIVEN
            List<string> lines = [];

            // WHEN
            List<FieldReference> result = ScriptParser.ParseFieldReferences(lines);

            // THEN
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        #endregion

        #region ReplaceFieldReferences Tests

        [Fact]
        public void GivenLineWithFieldReference_WhenReplacingFieldReferences_ThenReferenceIsReplaced()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = ["Value is {{Field1}}"];
            List<FieldReference> fieldReferences = [new(null, "Field1")];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].ShouldBe("Value is CurrentInstance.Field1");
        }

        [Fact]
        public void GivenLineWithInstanceFieldReference_WhenReplacingFieldReferences_ThenReferenceIsReplaced()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = ["Value is {{OtherInstance.Field1}}"];
            List<FieldReference> fieldReferences = [new("OtherInstance", "Field1")];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].ShouldBe("Value is OtherInstance.Field1");
        }

        [Fact]
        public void GivenLineWithMultipleFieldReferences_WhenReplacingFieldReferences_ThenAllReferencesAreReplaced()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = ["{{Field1}} and {{Field2}}"];
            List<FieldReference> fieldReferences = [new(null, "Field1"), new(null, "Field2")];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].ShouldBe("CurrentInstance.Field1 and CurrentInstance.Field2");
        }

        [Fact]
        public void GivenMultipleLines_WhenReplacingFieldReferences_ThenAllReferencesInAllLinesAreReplaced()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = ["{{Field1}}", "{{Field2}}", "{{Field3}}"];
            List<FieldReference> fieldReferences = [new(null, "Field1"), new(null, "Field2"), new(null, "Field3")];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(3);
            result[0].ShouldBe("CurrentInstance.Field1");
            result[1].ShouldBe("CurrentInstance.Field2");
            result[2].ShouldBe("CurrentInstance.Field3");
        }

        [Fact]
        public void GivenLineWithNoReferences_WhenReplacingFieldReferences_ThenLineIsUnchanged()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = ["Value is static"];
            List<FieldReference> fieldReferences = [new(null, "UnusedField")];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].ShouldBe("Value is static");
        }

        [Fact]
        public void GivenEmptyLines_WhenReplacingFieldReferences_ThenEmptyListIsReturned()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = [];
            List<FieldReference> fieldReferences = [new(null, "Field1")];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public void GivenEmptyFieldReferences_WhenReplacingFieldReferences_ThenLinesAreUnchanged()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = ["{{Field1}}"];
            List<FieldReference> fieldReferences = [];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].ShouldBe("{{Field1}}");
        }

        [Fact]
        public void GivenLineWithDuplicateFieldReferences_WhenReplacingFieldReferences_ThenAllAreReplaced()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = ["{{Field1}} equals {{Field1}}"];
            List<FieldReference> fieldReferences = [new(null, "Field1")];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].ShouldBe("CurrentInstance.Field1 equals CurrentInstance.Field1");
        }

        [Fact]
        public void GivenComplexLine_WhenReplacingFieldReferences_ThenAllReferencesInComplexLineAreReplaced()
        {
            // GIVEN
            const string ownInstance = "CurrentInstance";
            List<string> lines = ["if ({{Threshold}} < {{Value}} && {{OtherInstance.Status}} == OK)"];
            List<FieldReference> fieldReferences = [new(null, "Threshold"), new(null, "Value"), new("OtherInstance", "Status")];

            // WHEN
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result[0].ShouldBe("if (CurrentInstance.Threshold < CurrentInstance.Value && OtherInstance.Status == OK)");
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void GivenCompleteScript_WhenParsingAndReplacingReferences_ThenProcessIsCompletedSuccessfully()
        {
            // GIVEN
            const string script = "if {{Value}} > {{Threshold}}\nthen set {{Status}} to HIGH";
            const string ownInstance = "Sensor";

            // WHEN
            List<string> lines = ScriptParser.ParseLine(script);
            List<FieldReference> fieldReferences = ScriptParser.ParseFieldReferences(lines);
            List<string> result = ScriptParser.ReplaceFieldReferences(ownInstance, lines, fieldReferences);

            // THEN
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result[0].ShouldBe("if Sensor.Value > Sensor.Threshold");
            result[1].ShouldBe("then set Sensor.Status to HIGH");
            fieldReferences.Count.ShouldBe(3);
        }

        #endregion
    }
}