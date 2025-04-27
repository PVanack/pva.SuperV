using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Model.Services
{
    public interface IFieldFormatterService
    {
        Task<List<string>> GetFieldFormatterTypesAsync();
        Task<List<FieldFormatterModel>> GetFieldFormattersAsync(string projectId);
        Task<FieldFormatterModel> GetFieldFormatterAsync(string projectId, string fieldFormatterName);
        Task<FieldFormatterModel> CreateFieldFormatterAsync(string projectId, CreateFieldFormatterRequest createRequest);
        ValueTask DeleteFieldFormatterAsync(string projectId, string fieldFormatterName);
        Task<FieldFormatterModel> UpdateFieldFormatterAsync(string projectId, string fieldFormatterName, FieldFormatterModel fieldFormatterModel);
        Task<PagedSearchResult<FieldFormatterModel>> SearchFieldFormattersAsync(string projectId, FieldFormatterPagedSearchRequest search);
    }
}
