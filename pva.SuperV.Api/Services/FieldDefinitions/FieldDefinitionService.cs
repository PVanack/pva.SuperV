using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.FieldFormatters;
using pva.SuperV.Model;
using pva.SuperV.Model.FieldDefinitions;
using pva.SuperV.Model.Services;

namespace pva.SuperV.Api.Services.FieldDefinitions
{
    public class FieldDefinitionService : BaseService, IFieldDefinitionService
    {
        private readonly Dictionary<string, Comparison<FieldDefinitionModel>> sortOptions = new()
            {
                { "name", new Comparison<FieldDefinitionModel>((a, b) => a.Name.CompareTo(b.Name)) }
            };

        public async Task<List<FieldDefinitionModel>> GetFieldsAsync(string projectId, string className)
            => await Task.FromResult(GetClassEntity(projectId, className).FieldDefinitions.Values.Select(field => FieldDefinitionMapper.ToDto(field)).ToList());

        public async Task<FieldDefinitionModel> GetFieldAsync(string projectId, string className, string fieldName)
        {
            if (GetClassEntity(projectId, className).FieldDefinitions.TryGetValue(fieldName, out IFieldDefinition? fieldDefinition))
            {
                return await Task.FromResult(FieldDefinitionMapper.ToDto(fieldDefinition));
            }
            return await Task.FromException<FieldDefinitionModel>(new UnknownEntityException("Field", fieldName));
        }

        public async Task<PagedSearchResult<FieldDefinitionModel>> SearchFieldsAsync(string projectId, string className, FieldDefinitionPagedSearchRequest search)
        {
            List<FieldDefinitionModel> allFieldDefinitions = await GetFieldsAsync(projectId, className);
            List<FieldDefinitionModel> fieldDefinitions = FilterFieldDefinitions(allFieldDefinitions, search);
            fieldDefinitions = SortResult(fieldDefinitions, search.SortOption, sortOptions);
            return CreateResult(search, allFieldDefinitions, fieldDefinitions);
        }

        private static List<FieldDefinitionModel> FilterFieldDefinitions(List<FieldDefinitionModel> allFieldDefinitions, FieldDefinitionPagedSearchRequest search)
        {
            List<FieldDefinitionModel> filteredClasses = allFieldDefinitions;
            if (!String.IsNullOrEmpty(search.NameFilter))
            {
                filteredClasses = [.. filteredClasses.Where(fieldDefinition => fieldDefinition.Name.Contains(search.NameFilter))];
            }
            return filteredClasses;
        }


        public async Task<List<FieldDefinitionModel>> CreateFieldsAsync(string projectId, string className, List<FieldDefinitionModel> createRequests)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                List<FieldDefinitionModel> createdFieldDefinitions = [];
                Class clazz = GetClassEntity(wipProject, className);
                try
                {
                    createRequests.ForEach(fieldDefinition =>
                    {
                        FieldFormatter? fieldFormatter = null;
                        if (fieldDefinition.ValueFormatter is not null)
                        {
                            fieldFormatter = wipProject.GetFormatter(fieldDefinition.ValueFormatter);
                        }
                        createdFieldDefinitions.Add(FieldDefinitionMapper.ToDto(clazz.AddField(FieldDefinitionMapper.FromDto(fieldDefinition), fieldFormatter)));
                    });
                    return await Task.FromResult(createdFieldDefinitions);
                }
                catch (SuperVException e)
                {
                    // If exception while creatig one of the fields, remove all the already created fields.
                    try
                    {
                        createdFieldDefinitions.ForEach(fieldDefinition => clazz.RemoveField(fieldDefinition.Name));
                    }
                    catch (SuperVException)
                    {
                        // Ignore execption while deleting
                    }
                    return await Task.FromException<List<FieldDefinitionModel>>(e);
                }
            }
            return await Task.FromException<List<FieldDefinitionModel>>(new NonWipProjectException(projectId));
        }

        public async Task<FieldDefinitionModel> UpdateFieldAsync(string projectId, string className, string fieldName, FieldDefinitionModel updateRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                _ = GetClassEntity(wipProject, className);
                if (updateRequest.Name?.Equals(fieldName) != false)
                {
                    IFieldDefinition fieldDefinitionUpdate = FieldDefinitionMapper.FromDto(updateRequest);
                    return await Task.FromResult(FieldDefinitionMapper.ToDto(wipProject.UpdateField(className, fieldName, fieldDefinitionUpdate, updateRequest.ValueFormatter)));
                }
                return await Task.FromException<FieldDefinitionModel>(new EntityPropertyNotChangeableException("field", "Name"));
            }
            return await Task.FromException<FieldDefinitionModel>(new NonWipProjectException(projectId));
        }

        public async ValueTask DeleteFieldAsync(string projectId, string className, string fieldName)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                GetClassEntity(wipProject, className).RemoveField(fieldName);
                await ValueTask.CompletedTask;
                return;
            }
            await ValueTask.FromException(new NonWipProjectException(projectId));
        }
    }
}
