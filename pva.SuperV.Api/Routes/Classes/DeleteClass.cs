﻿
using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine.Exceptions;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class DeleteClass
    {
        internal static Results<NoContent, NotFound<string>, BadRequest<string>> Handle(IClassService classService, string projectId, string className)
        {
            try
            {
                classService.DeleteClass(projectId, className);
                return TypedResults.NoContent();
            }
            catch (UnknownEntityException e)
            {
                return TypedResults.NotFound<string>(e.Message);
            }
            catch (SuperVException e)
            {
                return TypedResults.BadRequest<string>(e.Message);
            }
        }
    }
}