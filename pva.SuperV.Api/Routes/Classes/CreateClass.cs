using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class CreateClass
    {
        internal static Results<Created<ClassModel>, NotFound<string>, InternalServerError<string>> Handle(IClassService classService, string projectId, ClassModel createRequest)
        {
            try
            {
                ClassModel createdClass = classService.CreateClass(projectId, createRequest);
                return TypedResults.Created($"/classes/{projectId}/{createdClass.Name}", createdClass);
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