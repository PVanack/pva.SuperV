
using pva.SuperV.Model;

namespace pva.SuperV.Api
{
    public interface IFieldFormatterService
    {
        List<string> GetFieldFormatterTypes();
        List<FieldFormatterModel> GetFieldFormatters(string projectId);
        FieldFormatterModel GetFieldFormatter(string projectId, string fieldFormatterName);
        FieldFormatterModel CreateFieldFormatter(string projectId, FieldFormatterModel fieldFormatter);
    }
}
