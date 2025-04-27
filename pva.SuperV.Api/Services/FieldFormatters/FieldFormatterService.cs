using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.FieldFormatters
{
    public class FieldFormatterService : BaseService, IFieldFormatterService
    {
        private readonly Dictionary<string, Comparison<FieldFormatterModel>> sortOptions = new()
            {
                { "name", new Comparison<FieldFormatterModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        public async Task<List<string>> GetFieldFormatterTypesAsync()
        {
            Type fieldFormatterType = typeof(FieldFormatter);
            return await Task.FromResult(AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetExportedTypes())
                .Where(type => type.IsSubclassOf(fieldFormatterType) &&
                        type != fieldFormatterType && !type.IsAbstract)
                .Select(type => type.ToString())
                .ToList());
        }

        public async Task<List<FieldFormatterModel>> GetFieldFormattersAsync(string projectId)
        {
            return await Task.FromResult(GetProjectEntity(projectId).FieldFormatters.Values
                .Select(fieldFormatter => FieldFormatterMapper.ToDto(fieldFormatter))
                .ToList());
        }

        public async Task<PagedSearchResult<FieldFormatterModel>> SearchFieldFormattersAsync(string projectId, FieldFormatterPagedSearchRequest search)
        {
            List<FieldFormatterModel> allFieldFormatters = await GetFieldFormattersAsync(projectId);
            List<FieldFormatterModel> fieldFormatters = FilterFieldFormatters(allFieldFormatters, search);
            fieldFormatters = SortResult(fieldFormatters, search.SortOption, sortOptions);
            return CreateResult(search, allFieldFormatters, fieldFormatters);

        }

        private static List<FieldFormatterModel> FilterFieldFormatters(List<FieldFormatterModel> allFieldFormatters, FieldFormatterPagedSearchRequest search)
        {
            List<FieldFormatterModel> filteredFieldDefinitions = allFieldFormatters;
            if (!String.IsNullOrEmpty(search.NameFilter))
            {
                filteredFieldDefinitions = [.. filteredFieldDefinitions.Where(clazz => clazz.Name.Contains(search.NameFilter))];
            }
            return filteredFieldDefinitions;
        }

        public async Task<FieldFormatterModel> GetFieldFormatterAsync(string projectId, string fieldFormatterName)
        {
            if (GetProjectEntity(projectId).FieldFormatters.TryGetValue(fieldFormatterName, out FieldFormatter? fieldFormatter))
            {
                return await Task.FromResult(FieldFormatterMapper.ToDto(fieldFormatter));
            }
            return await Task.FromException<FieldFormatterModel>(new UnknownEntityException("Field formatter", fieldFormatterName));
        }

        public async Task<FieldFormatterModel> CreateFieldFormatterAsync(string projectId, CreateFieldFormatterRequest createRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                FieldFormatter fieldFormatter = FieldFormatterMapper.FromDto(createRequest.FieldFormatter);
                wipProject.AddFieldFormatter(fieldFormatter);
                return await Task.FromResult(FieldFormatterMapper.ToDto(fieldFormatter));
            }
            return await Task.FromException<FieldFormatterModel>(new NonWipProjectException(projectId));
        }

        public async Task<FieldFormatterModel> UpdateFieldFormatterAsync(string projectId, string fieldFormatterName, FieldFormatterModel fieldFormatterModel)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                FieldFormatter fieldFormatter = FieldFormatterMapper.FromDto(fieldFormatterModel);
                wipProject.UpdateFieldFormatter(fieldFormatterName, fieldFormatter);
                return await Task.FromResult(FieldFormatterMapper.ToDto(fieldFormatter));
            }
            return await Task.FromException<FieldFormatterModel>(new NonWipProjectException(projectId));
        }

        public async ValueTask DeleteFieldFormatterAsync(string projectId, string fieldFormatterName)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                if (wipProject.RemoveFieldFormatter(fieldFormatterName))
                {
                    await ValueTask.CompletedTask;
                    return;
                }
                await ValueTask.FromException(new UnknownEntityException("Field formatter", fieldFormatterName));
            }
            await ValueTask.FromException(new NonWipProjectException(projectId));
        }

    }
}
