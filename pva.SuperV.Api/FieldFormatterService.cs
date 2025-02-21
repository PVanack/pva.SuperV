using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Model.FieldFormatters;

namespace pva.SuperV.Api
{
    public class FieldFormatterService : BaseService, IFieldFormatterService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        public List<string> GetFieldFormatterTypes()
        {
            Type fieldFormatterType = typeof(FieldFormatter);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(domainAssembly => domainAssembly.GetExportedTypes())
                .Where(type => type.IsSubclassOf(fieldFormatterType) &&
                        type != fieldFormatterType && !type.IsAbstract)
                .Select(type => type.ToString())
                .ToList();
        }

        public List<FieldFormatterModel> GetFieldFormatters(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return project.FieldFormatters.Values
                .Select(fieldFormatter => FieldFormatterMapper.ToDto(fieldFormatter))
                .ToList();
        }

        public FieldFormatterModel GetFieldFormatter(string projectId, string fieldFormatterName)
        {
            Project project = GetProjectEntity(projectId);
            if (project.FieldFormatters.TryGetValue(fieldFormatterName, out FieldFormatter? fieldFormatter))
            {
                return FieldFormatterMapper.ToDto(fieldFormatter);
            }
            throw new UnknownEntityException("Field formatter", fieldFormatterName);
        }

        public FieldFormatterModel CreateFieldFormatter(string projectId, FieldFormatterModel fieldFormatterModel)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
            {
                FieldFormatter fieldFormatter = FieldFormatterMapper.FromDto(fieldFormatterModel);
                wipProject.AddFieldFormatter(fieldFormatter);
                return FieldFormatterMapper.ToDto(fieldFormatter);
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteFieldFormatter(string projectId, string fieldFormatterName)
        {
            Project project = GetProjectEntity(projectId);
            if (project is WipProject wipProject)
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
