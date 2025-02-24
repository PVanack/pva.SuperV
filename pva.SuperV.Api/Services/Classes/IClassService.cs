using pva.SuperV.Engine;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api.Services.Classes
{
    public interface IClassService
    {
        List<ClassModel> GetClasses(string projectId);
        ClassModel GetClass(string projectId, string className);
        ClassModel CreateClass(string projectId, ClassModel createRequest);
        void DeleteClass(string projectId, string className);
    }
}
