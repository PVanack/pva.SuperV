﻿using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    internal static class GetFieldFormatters
    {
        internal static Results<Ok<List<FieldFormatterModel>>, NotFound<string>, BadRequest<string>> Handle(IFieldFormatterService fieldFormatterService, string projectId)
        {
            try
            {
                return TypedResults.Ok(fieldFormatterService.GetFieldFormatters(projectId));
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
