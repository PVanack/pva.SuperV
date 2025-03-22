﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by Reqnroll (https://www.reqnroll.net/).
//      Reqnroll Version:2.0.0.0
//      Reqnroll Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace pva.SuperV.TestsScenarios.Features
{
    using Reqnroll;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Reqnroll", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class ProjectFeature
    {
        
        private global::Reqnroll.ITestRunner testRunner;
        
        private Microsoft.VisualStudio.TestTools.UnitTesting.TestContext _testContext;
        
        private static string[] featureTags = ((string[])(null));
        
        private static global::Reqnroll.FeatureInfo featureInfo = new global::Reqnroll.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "Project", null, global::Reqnroll.ProgrammingLanguage.CSharp, featureTags);
        
#line 1 "Project.feature"
#line hidden
        
        public virtual Microsoft.VisualStudio.TestTools.UnitTesting.TestContext TestContext
        {
            get
            {
                return this._testContext;
            }
            set
            {
                this._testContext = value;
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static async System.Threading.Tasks.Task FeatureSetupAsync(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute(Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupBehavior.EndOfClass)]
        public static async System.Threading.Tasks.Task FeatureTearDownAsync()
        {
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public async System.Threading.Tasks.Task TestInitializeAsync()
        {
            testRunner = global::Reqnroll.TestRunnerManager.GetTestRunnerForAssembly(featureHint: featureInfo);
            if (((testRunner.FeatureContext != null) 
                        && (testRunner.FeatureContext.FeatureInfo.Equals(featureInfo) == false)))
            {
                await testRunner.OnFeatureEndAsync();
            }
            if ((testRunner.FeatureContext == null))
            {
                await testRunner.OnFeatureStartAsync(featureInfo);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public async System.Threading.Tasks.Task TestTearDownAsync()
        {
            await testRunner.OnScenarioEndAsync();
            global::Reqnroll.TestRunnerManager.ReleaseTestRunner(testRunner);
        }
        
        public void ScenarioInitialize(global::Reqnroll.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Microsoft.VisualStudio.TestTools.UnitTesting.TestContext>(_testContext);
        }
        
        public async System.Threading.Tasks.Task ScenarioStartAsync()
        {
            await testRunner.OnScenarioStartAsync();
        }
        
        public async System.Threading.Tasks.Task ScenarioCleanupAsync()
        {
            await testRunner.CollectScenarioErrorsAsync();
        }
        
        public virtual async System.Threading.Tasks.Task FeatureBackgroundAsync()
        {
#line 3
#line hidden
#line 4
 await testRunner.GivenAsync("REST application is started", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Create project")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Project")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("mytag")]
        public async System.Threading.Tasks.Task CreateProject()
        {
            string[] tagsOfScenario = new string[] {
                    "mytag"};
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            global::Reqnroll.ScenarioInfo scenarioInfo = new global::Reqnroll.ScenarioInfo("Create project", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 7
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((global::Reqnroll.TagHelper.ContainsIgnoreTag(scenarioInfo.CombinedTags) || global::Reqnroll.TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                await this.ScenarioStartAsync();
#line 3
await this.FeatureBackgroundAsync();
#line hidden
#line 8
 await testRunner.GivenAsync("An empty WIP project \"Project\" is created  with \"A new project\" description and \"" +
                        "TDengine\" as history storage", ((string)(null)), ((global::Reqnroll.Table)(null)), "Given ");
#line hidden
#line 9
 await testRunner.AndAsync("History repository \"HistoryRepository\" is created in project \"Project-WIP\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
                global::Reqnroll.Table table1 = new global::Reqnroll.Table(new string[] {
                            "Value",
                            "String"});
                table1.AddRow(new string[] {
                            "-2",
                            "Low low"});
                table1.AddRow(new string[] {
                            "-1",
                            "Low"});
                table1.AddRow(new string[] {
                            "0",
                            "OK"});
                table1.AddRow(new string[] {
                            "1",
                            "High"});
                table1.AddRow(new string[] {
                            "2",
                            "High high"});
#line 10
 await testRunner.AndAsync("Enum formatter \"AlarmStates\" is created in project \"Project-WIP\"", ((string)(null)), table1, "And ");
#line hidden
                global::Reqnroll.Table table2 = new global::Reqnroll.Table(new string[] {
                            "Value",
                            "String"});
                table2.AddRow(new string[] {
                            "0",
                            "Ack"});
                table2.AddRow(new string[] {
                            "1",
                            "Unack"});
#line 17
 await testRunner.AndAsync("Enum formatter \"AckStates\" is created in project \"Project-WIP\"", ((string)(null)), table2, "And ");
#line hidden
                global::Reqnroll.Table table3 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Default value",
                            "Format"});
#line 21
 await testRunner.AndAsync("Class \"BaseClass\" is created in project \"Project-WIP\" with the following fields", ((string)(null)), table3, "And ");
#line hidden
                global::Reqnroll.Table table4 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Default value",
                            "Format"});
                table4.AddRow(new string[] {
                            "Value",
                            "double",
                            "50",
                            ""});
                table4.AddRow(new string[] {
                            "HighHighLimit",
                            "double",
                            "100",
                            ""});
                table4.AddRow(new string[] {
                            "HighLimit",
                            "double",
                            "75",
                            ""});
                table4.AddRow(new string[] {
                            "LowLimit",
                            "double",
                            "25",
                            ""});
                table4.AddRow(new string[] {
                            "LowLowLimit",
                            "double",
                            "0",
                            ""});
                table4.AddRow(new string[] {
                            "AlarmState",
                            "int",
                            "0",
                            "AlarmStates"});
                table4.AddRow(new string[] {
                            "AckState",
                            "int",
                            "0",
                            "AckStates"});
#line 23
 await testRunner.AndAsync("Class \"TheClass\" with base class \"BaseClass\" is created in project \"Project-WIP\" " +
                        "with the following fields", ((string)(null)), table4, "And ");
#line hidden
                global::Reqnroll.Table table5 = new global::Reqnroll.Table(new string[] {
                            "HighHigh limit field",
                            "High limit field",
                            "Low limit field",
                            "LowLow limit field",
                            "Deadband field",
                            "AlarmState field",
                            "AckState field"});
                table5.AddRow(new string[] {
                            "HighHighLimit",
                            "HighLimit",
                            "LowLimit",
                            "LowLowLimit",
                            "",
                            "AlarmState",
                            "AckState"});
#line 32
 await testRunner.AndAsync("Alarm state processing \"AlarmState\" is created on field \"Value\" of class \"TheClas" +
                        "s\" of project \"Project-WIP\"", ((string)(null)), table5, "And ");
#line hidden
                global::Reqnroll.Table table6 = new global::Reqnroll.Table(new string[] {
                            "History repository",
                            "Timestamp field",
                            "Field to historize"});
                table6.AddRow(new string[] {
                            "HistoryRepository",
                            "",
                            ""});
                table6.AddRow(new string[] {
                            "",
                            "",
                            "Value"});
                table6.AddRow(new string[] {
                            "",
                            "",
                            "AlarmState"});
                table6.AddRow(new string[] {
                            "",
                            "",
                            "AckState"});
#line 35
 await testRunner.AndAsync("Historization processing \"ValueHistorization\" is created on field \"Value\" of clas" +
                        "s \"TheClass\" of project \"Project-WIP\"", ((string)(null)), table6, "And ");
#line hidden
                global::Reqnroll.Table table7 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Default value",
                            "Format"});
                table7.AddRow(new string[] {
                            "Bool",
                            "bool",
                            "true",
                            ""});
                table7.AddRow(new string[] {
                            "DateTime",
                            "DateTime",
                            "2025-03-01T00:00:00Z",
                            ""});
                table7.AddRow(new string[] {
                            "Double",
                            "double",
                            "12.3",
                            ""});
                table7.AddRow(new string[] {
                            "Float",
                            "float",
                            "3.21",
                            ""});
                table7.AddRow(new string[] {
                            "Int",
                            "int",
                            "134567",
                            ""});
                table7.AddRow(new string[] {
                            "Long",
                            "long",
                            "9876543",
                            ""});
                table7.AddRow(new string[] {
                            "Short",
                            "short",
                            "32767",
                            ""});
                table7.AddRow(new string[] {
                            "String",
                            "string",
                            "Hi from SuperV!",
                            ""});
                table7.AddRow(new string[] {
                            "TimeSpan",
                            "TimeSpan",
                            "01:02:59",
                            ""});
                table7.AddRow(new string[] {
                            "Uint",
                            "uint",
                            "123456",
                            ""});
                table7.AddRow(new string[] {
                            "Ulong",
                            "ulong",
                            "98123456",
                            ""});
                table7.AddRow(new string[] {
                            "Ushort",
                            "ushort",
                            "32767",
                            ""});
#line 42
 await testRunner.AndAsync("Class \"AllFieldsClass\" is created in project \"Project-WIP\" with the following fie" +
                        "lds", ((string)(null)), table7, "And ");
#line hidden
#line 56
 await testRunner.AndAsync("Runnable project is built from WIP project \"Project-WIP\"", ((string)(null)), ((global::Reqnroll.Table)(null)), "And ");
#line hidden
                global::Reqnroll.Table table8 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Value"});
                table8.AddRow(new string[] {
                            "Value",
                            "double",
                            "50"});
#line 57
 await testRunner.AndAsync("Instance \"AnInstance\" is created with class \"TheClass\" in project \"Project\"", ((string)(null)), table8, "And ");
#line hidden
                global::Reqnroll.Table table9 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Value"});
                table9.AddRow(new string[] {
                            "Value",
                            "double",
                            "50"});
                table9.AddRow(new string[] {
                            "HighHighLimit",
                            "double",
                            "99"});
                table9.AddRow(new string[] {
                            "LowLowLimit",
                            "double",
                            "1"});
                table9.AddRow(new string[] {
                            "AckState",
                            "string",
                            "Unack"});
#line 60
 await testRunner.AndAsync("Instance \"AnInstance\" fields values are updated in project \"Project\"", ((string)(null)), table9, "And ");
#line hidden
                global::Reqnroll.Table table10 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Value",
                            "Formatted value"});
                table10.AddRow(new string[] {
                            "Value",
                            "double",
                            "50",
                            ""});
                table10.AddRow(new string[] {
                            "HighHighLimit",
                            "double",
                            "99",
                            ""});
                table10.AddRow(new string[] {
                            "LowLowLimit",
                            "double",
                            "1",
                            ""});
                table10.AddRow(new string[] {
                            "AckState",
                            "int",
                            "1",
                            "Unack"});
#line 67
 await testRunner.ThenAsync("Instance \"AnInstance\" fields have expected values in project \"Project\"", ((string)(null)), table10, "Then ");
#line hidden
                global::Reqnroll.Table table11 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Value"});
#line 74
 await testRunner.GivenAsync("Instance \"AllFieldsInstance\" is created with class \"AllFieldsClass\" in project \"P" +
                        "roject\"", ((string)(null)), table11, "Given ");
#line hidden
                global::Reqnroll.Table table12 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Value"});
                table12.AddRow(new string[] {
                            "Bool",
                            "bool",
                            "true"});
                table12.AddRow(new string[] {
                            "DateTime",
                            "DateTime",
                            "2025-03-01T00:00:00Z"});
                table12.AddRow(new string[] {
                            "Double",
                            "double",
                            "12.3"});
                table12.AddRow(new string[] {
                            "Float",
                            "float",
                            "3.21"});
                table12.AddRow(new string[] {
                            "Int",
                            "int",
                            "134567"});
                table12.AddRow(new string[] {
                            "Long",
                            "long",
                            "9876543"});
                table12.AddRow(new string[] {
                            "Short",
                            "short",
                            "32767"});
                table12.AddRow(new string[] {
                            "String",
                            "string",
                            "Hi from SuperV!"});
                table12.AddRow(new string[] {
                            "TimeSpan",
                            "TimeSpan",
                            "01:02:59"});
                table12.AddRow(new string[] {
                            "Uint",
                            "uint",
                            "123456"});
                table12.AddRow(new string[] {
                            "Ulong",
                            "ulong",
                            "98123456"});
                table12.AddRow(new string[] {
                            "Ushort",
                            "ushort",
                            "32767"});
#line 77
 await testRunner.AndAsync("Instance \"AllFieldsInstance\" fields values are updated in project \"Project\"", ((string)(null)), table12, "And ");
#line hidden
                global::Reqnroll.Table table13 = new global::Reqnroll.Table(new string[] {
                            "Name",
                            "Type",
                            "Value"});
                table13.AddRow(new string[] {
                            "Bool",
                            "bool",
                            "true"});
                table13.AddRow(new string[] {
                            "DateTime",
                            "DateTime",
                            "2025-03-01T00:00:00Z"});
                table13.AddRow(new string[] {
                            "Double",
                            "double",
                            "12.3"});
                table13.AddRow(new string[] {
                            "Float",
                            "float",
                            "3.21"});
                table13.AddRow(new string[] {
                            "Int",
                            "int",
                            "134567"});
                table13.AddRow(new string[] {
                            "Long",
                            "long",
                            "9876543"});
                table13.AddRow(new string[] {
                            "Short",
                            "short",
                            "32767"});
                table13.AddRow(new string[] {
                            "String",
                            "string",
                            "Hi from SuperV!"});
                table13.AddRow(new string[] {
                            "TimeSpan",
                            "TimeSpan",
                            "01:02:59"});
                table13.AddRow(new string[] {
                            "Uint",
                            "uint",
                            "123456"});
                table13.AddRow(new string[] {
                            "Ulong",
                            "ulong",
                            "98123456"});
                table13.AddRow(new string[] {
                            "Ushort",
                            "ushort",
                            "32767"});
#line 92
 await testRunner.ThenAsync("Instance \"AllFieldsInstance\" fields have expected values in project \"Project\"", ((string)(null)), table13, "Then ");
#line hidden
            }
            await this.ScenarioCleanupAsync();
        }
    }
}
#pragma warning restore
#endregion
