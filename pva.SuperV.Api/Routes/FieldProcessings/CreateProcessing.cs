﻿using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldProcessings;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldProcessings
{
    internal static class CreateProcessing
    {
        internal static async Task<Results<Created<FieldValueProcessingModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldProcessingService fieldProcessingService, string projectId, string className, string fieldName, FieldValueProcessingModel createRequest)
        {
            try
            {
                FieldValueProcessingModel createdFieldProcessing = await fieldProcessingService.CreateFieldProcessingAsync(projectId, className, fieldName, createRequest);
                return TypedResults.Created<FieldValueProcessingModel>($"//field-processings/{projectId}/{className}/{fieldName}{createdFieldProcessing.Name}", createdFieldProcessing);
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest(e.Message);
            }
        }
    }
}