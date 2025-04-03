using Microsoft.AspNetCore.Http.HttpResults;
using pva.SuperV.Api.Services.Classes;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Routes.Classes
{
    internal static class UpdateClass
    {
        internal static Results<Ok<ClassModel>, NotFound<string>, BadRequest<string>> Handle(IClassService classService, string wipProjectId, string className, ClassModel updateRequest)
        {
            try
            {
                ClassModel updatedClass = classService.UpdateClass(wipProjectId, className, updateRequest);
                return TypedResults.Ok(updatedClass);
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