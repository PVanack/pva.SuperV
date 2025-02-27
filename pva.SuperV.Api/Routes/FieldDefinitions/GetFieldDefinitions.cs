﻿using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldDefinitions;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldDefinitions;

namespace pva.SuperV.Api.Routes.FieldDefinitions
{
    internal class GetFieldDefinitions
    {
        internal static Results<Ok<List<FieldDefinitionModel>>, NotFound<string>, InternalServerError<string>> Handle(IFieldDefinitionService fieldDefinitionService, string projectId, string className)
        {
            try
            {
                return TypedResults.Ok(fieldDefinitionService.GetFields(projectId, className));
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }
    }
}