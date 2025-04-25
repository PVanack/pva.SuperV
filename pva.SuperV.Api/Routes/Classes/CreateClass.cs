using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class CreateClass
    {
        internal static async Task<Results<Created<ClassModel>, NotFound<string>, BadRequest<string>>>
            Handle(IClassService classService, string projectId, ClassModel createRequest)
        {
            try
            {
                ClassModel createdClass = await classService.CreateClassAsync(projectId, createRequest);
                return TypedResults.Created($"/classes/{projectId}/{createdClass.Name}", createdClass);
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