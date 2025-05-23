﻿using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class GetFieldFormatter
    {
        internal static async Task<Results<Ok<FieldFormatterModel>, NotFound<string>, BadRequest<string>>>
            Handle(IFieldFormatterService fieldFormatterService, string projectId, string fieldFormatterName)
        {
            try
            {
                FieldFormatterModel fieldFormatter = await fieldFormatterService.GetFieldFormatterAsync(projectId, fieldFormatterName);
                return TypedResults.Ok<FieldFormatterModel>(fieldFormatter);
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
