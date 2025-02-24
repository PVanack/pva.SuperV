﻿using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.FieldFormatters;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.FieldFormatters
{
    public static class GetFieldFormatterTypes
    {
        internal static Results<Ok<List<string>>, InternalServerError<string>> Handle(IFieldFormatterService fieldFormatterService)
        {
            try
            {
                List<string> formatterTypes = fieldFormatterService.GetFieldFormatterTypes();
                return TypedResults.Ok<List<string>>(formatterTypes);
            }
            catch (SuperVException e)
            {
                return TypedResults.InternalServerError(e.Message);
            }
        }

    }
}
