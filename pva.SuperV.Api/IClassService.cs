using pva.SuperV.Engine;
using pva.SuperV.Model.Classes;

namespace pva.SuperV.Api
{
    public interface IClassService
    {
        List<ClassModel> GetClasses(string projectId);
        ClassModel GetClass(string projectId, string className);
        ClassModel CreateClass(string projectId, ClassModel createRequest);
    }
}
