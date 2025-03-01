﻿using Docker.DotNet.Models;
using pva.Helpers.Extensions;
using pva.SuperV.Api.Services.Instances;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.EngineTests;
using pva.SuperV.Model.Instances;
using Shouldly;

namespace pva.SuperV.ApiTests
{
    [Collection("Project building")]
    public class InstanceServiceTests
    {
        private readonly InstanceService _instanceService;
        private readonly RunnableProject runnableProject;
        private readonly WipProject wipProject;
        private InstanceModel expectedInstance;

        public InstanceServiceTests()
        {
            _instanceService = new();
            runnableProject = ProjectHelpers.CreateRunnableProject();
            Instance? instance = runnableProject.CreateInstance(ProjectHelpers.ClassName, ProjectHelpers.InstanceName);
            expectedInstance = new(instance!.Name, instance!.Class.Name,
            [
                new FieldModel(ProjectHelpers.BaseClassFieldName, typeof(string).ToString(), FieldValueMapper.ToDto(instance!.GetField(ProjectHelpers.BaseClassFieldName))),
                new FieldModel(ProjectHelpers.ValueFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(ProjectHelpers.ValueFieldName))),
                new FieldModel(ProjectHelpers.HighHighLimitFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(ProjectHelpers.HighHighLimitFieldName))),
                new FieldModel(ProjectHelpers.HighLimitFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(ProjectHelpers.HighLimitFieldName))),
                new FieldModel(ProjectHelpers.LowLimitFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(ProjectHelpers.LowLimitFieldName))),
                new FieldModel(ProjectHelpers.LowLowLimitFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(ProjectHelpers.LowLowLimitFieldName))),
                new FieldModel(ProjectHelpers.AlarmStateFieldName, typeof(int).ToString(), FieldValueMapper.ToDto(instance!.GetField(ProjectHelpers.AlarmStateFieldName)))
            ]);
            wipProject = ProjectHelpers.CreateWipProject(null);
        }

        [Fact]
        public void GetInstances_ShouldReturnListOfInstances()
        {
            List<InstanceModel> expectedInstances = [expectedInstance];
            // Act
            var result = _instanceService.GetInstances(runnableProject.GetId());

            // Assert
            result.ShouldBeEquivalentTo(expectedInstances);
        }

        [Fact]
        public void GetInstance_ShouldReturnInstance_WhenInstanceExists()
        {
            // Act
            var result = _instanceService.GetInstance(runnableProject.GetId(), ProjectHelpers.InstanceName);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEquivalentTo(expectedInstance);
        }

        [Fact]
        public void GetInstance_ShouldThrowUnknownEntityException_WhenInstanceDoesNotExist()
        {
            // Act & Assert
            Assert.Throws<UnknownEntityException>(()
                => _instanceService.GetInstance(runnableProject.GetId(), "UnknownInstance"));
        }

        [Fact]
        public void CreateInstanceInRunnableProject_ShouldCreateInstance()
        {
            InstanceModel expectedCreatedInstance = expectedInstance with { Name = "Instance1" };
            // Act & Assert
            InstanceModel createInstanceModel = _instanceService.CreateInstance(runnableProject.GetId(), expectedCreatedInstance.ClassName, expectedCreatedInstance.Name);

            createInstanceModel.Fields.Count.ShouldBe(expectedCreatedInstance.Fields.Count);
            List<FieldModel> fieldsUpdatedWithTimestampsAndQualities = [];
            for (int index = 0; index < createInstanceModel.Fields.Count; index++)
            {
                fieldsUpdatedWithTimestampsAndQualities.Add(expectedCreatedInstance.Fields[index]
                    with
                    {
                        FieldValue = expectedCreatedInstance.Fields[index].FieldValue
                            with
                            {
                                Quality = createInstanceModel.Fields[index].FieldValue.Quality,
                                Timestamp = createInstanceModel.Fields[index].FieldValue.Timestamp
                            }
                    }
                );
            }
            expectedCreatedInstance = expectedCreatedInstance with { Fields = fieldsUpdatedWithTimestampsAndQualities };

            createInstanceModel.ShouldNotBeNull()
                .ShouldBeEquivalentTo(expectedCreatedInstance);
        }

        [Fact]
        public void DeleteInstanceInRunnableProject_ShouldDeleteInstance()
        {
            // Act
            _instanceService.DeleteInstance(runnableProject.GetId(), expectedInstance.Name);

            // Assert
            runnableProject.Instances.ShouldNotContainKey(expectedInstance.Name);
        }
    }
}
