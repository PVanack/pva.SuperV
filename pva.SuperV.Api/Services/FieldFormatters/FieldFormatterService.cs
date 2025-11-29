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
        private readonly ILogger logger;
        private readonly Dictionary<string, Comparison<FieldFormatterModel>> sortOptions = new()
            {
                { "name", new Comparison<FieldFormatterModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public FieldFormatterService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task<List<string>> GetFieldFormatterTypesAsync()
        {
            logger.LogDebug("Getting field formatters types");
            Type fieldFormatterType = typeof(FieldFormatter);
#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
            return await Task.FromResult(AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetExportedTypes())
                .Where(type => type.IsSubclassOf(fieldFormatterType) &&
                        type != fieldFormatterType && !type.IsAbstract)
                .Select(type => type.ToString())
                .ToList());
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        }

        public async Task<List<FieldFormatterModel>> GetFieldFormattersAsync(string projectId)
        {
            logger.LogDebug("Getting field formatters for project {ProjectId}",
                projectId);
            return await Task.FromResult(GetProjectEntity(projectId).FieldFormatters.Values
                .Select(fieldFormatter => FieldFormatterMapper.ToDto(fieldFormatter))
                .ToList());
        }
        public async Task<PagedSearchResult<FieldFormatterModel>> SearchFieldFormattersAsync(string projectId, FieldFormatterPagedSearchRequest search)
        {
            logger.LogDebug("Searching field formatters for project {ProjectId} with filter {NameFilter} page number {PageNumber} page size {PageSize}",
               projectId, search.NameFilter, search.PageNumber, search.PageSize);
            List<FieldFormatterModel> allFieldFormatters = await GetFieldFormattersAsync(projectId);
            List<FieldFormatterModel> fieldFormatters = FilterFieldFormatters(allFieldFormatters, search);
            fieldFormatters = SortResult(fieldFormatters, search.SortOption, sortOptions);
            return CreateResult(search, allFieldFormatters, fieldFormatters);

        }

        public async Task<FieldFormatterModel> GetFieldFormatterAsync(string projectId, string fieldFormatterName)
        {
            logger.LogDebug("Getting field formatter {FieldFormatterName} for project {ProjectId}",
                fieldFormatterName, projectId);
            if (GetProjectEntity(projectId).FieldFormatters.TryGetValue(fieldFormatterName, out FieldFormatter? fieldFormatter))
            {
                return await Task.FromResult(FieldFormatterMapper.ToDto(fieldFormatter));
            }
            return await Task.FromException<FieldFormatterModel>(new UnknownEntityException("Field formatter", fieldFormatterName));
        }


        public async Task<FieldFormatterModel> CreateFieldFormatterAsync(string projectId, CreateFieldFormatterRequest createRequest)
        {
            logger.LogDebug("Creating field formatter {FieldFormatterName} of type {FieldFormatterType} for project {ProjectId}",
                createRequest.FieldFormatter.Name, createRequest.FieldFormatter.FormatterType, projectId);
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
            logger.LogDebug("Uodating field formatter {FieldFormatterName} of type {FieldFormatterType} for project {ProjectId}",
                fieldFormatterName, fieldFormatterModel.FormatterType, projectId);
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
            logger.LogDebug("Deleting field formatter {FieldFormatterName} for project {ProjectId}",
                fieldFormatterName, projectId);
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

        private static List<FieldFormatterModel> FilterFieldFormatters(List<FieldFormatterModel> allFieldFormatters, FieldFormatterPagedSearchRequest search)
        {
            List<FieldFormatterModel> filteredFieldDefinitions = allFieldFormatters;
            if (!String.IsNullOrEmpty(search.NameFilter))
            {
                filteredFieldDefinitions = [.. filteredFieldDefinitions.Where(clazz => clazz.Name.Contains(search.NameFilter))];
            }
            return filteredFieldDefinitions;
        }
    }
}

