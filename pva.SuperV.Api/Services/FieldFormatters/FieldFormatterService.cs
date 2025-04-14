using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldFormatters;

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
        public List<string> GetFieldFormatterTypes()
        {
            Type fieldFormatterType = typeof(FieldFormatter);
            return [.. AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetExportedTypes())
                .Where(type => type.IsSubclassOf(fieldFormatterType) &&
                        type != fieldFormatterType && !type.IsAbstract)
                .Select(type => type.ToString())];
        }

        public List<FieldFormatterModel> GetFieldFormatters(string projectId)
        {
            return [.. GetProjectEntity(projectId).FieldFormatters.Values.Select(fieldFormatter => FieldFormatterMapper.ToDto(fieldFormatter))];
        }

        public PagedSearchResult<FieldFormatterModel> SearchFieldFormatters(string projectId, FieldFormatterPagedSearchRequest search)
        {
            List<FieldFormatterModel> allFieldFormatters = GetFieldFormatters(projectId);
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

        public FieldFormatterModel GetFieldFormatter(string projectId, string fieldFormatterName)
        {
            if (GetProjectEntity(projectId).FieldFormatters.TryGetValue(fieldFormatterName, out FieldFormatter? fieldFormatter))
            {
                return FieldFormatterMapper.ToDto(fieldFormatter);
            }
            throw new UnknownEntityException("Field formatter", fieldFormatterName);
        }

        public FieldFormatterModel CreateFieldFormatter(string projectId, FieldFormatterModel fieldFormatterModel)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                FieldFormatter fieldFormatter = FieldFormatterMapper.FromDto(fieldFormatterModel);
                wipProject.AddFieldFormatter(fieldFormatter);
                return FieldFormatterMapper.ToDto(fieldFormatter);
            }
            throw new NonWipProjectException(projectId);
        }

        public FieldFormatterModel UpdateFieldFormatter(string projectId, string fieldFormatterName, FieldFormatterModel fieldFormatterModel)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                FieldFormatter fieldFormatter = FieldFormatterMapper.FromDto(fieldFormatterModel);
                wipProject.UpdateFieldFormatter(fieldFormatterName, fieldFormatter);
                return FieldFormatterMapper.ToDto(fieldFormatter);
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteFieldFormatter(string projectId, string fieldFormatterName)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                if (wipProject.RemoveFieldFormatter(fieldFormatterName))
                {
                    return;
                }
                throw new UnknownEntityException("Field formatter", fieldFormatterName);
            }
            throw new NonWipProjectException(projectId);
        }

    }
}
