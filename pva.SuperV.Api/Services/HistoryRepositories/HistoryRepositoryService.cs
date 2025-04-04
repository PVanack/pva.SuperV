﻿using pva.SuperV.Api.Exceptions;
using pva.SuperV.Engine;
using pva.SuperV.Engine.Exceptions;
using pva.SuperV.Engine.HistoryStorage;
using pva.SuperV.Model.HistoryRepositories;

namespace pva.SuperV.Api.Services.HistoryRepositories
{
    public class HistoryRepositoryService : BaseService, IHistoryRepositoryService
    {
        public List<HistoryRepositoryModel> GetHistoryRepositories(string projectId)
        {
            Project project = GetProjectEntity(projectId);
            return [.. project.HistoryRepositories.Values.Select(HistoryRepositoryMapper.ToDto)];
        }

        public HistoryRepositoryModel GetHistoryRepository(string projectId, string historyRepositoryName)
        {
            if (GetProjectEntity(projectId).HistoryRepositories.TryGetValue(historyRepositoryName, out HistoryRepository? historyRepository))
            {
                return HistoryRepositoryMapper.ToDto(historyRepository);
            }
            throw new UnknownEntityException("History repository", historyRepositoryName);
        }

        public HistoryRepositoryModel CreateHistoryRepository(string projectId, HistoryRepositoryModel historyRepositoryCreateRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                HistoryRepository historyRepository = HistoryRepositoryMapper.FromDto(historyRepositoryCreateRequest);
                wipProject.AddHistoryRepository(historyRepository);
                return HistoryRepositoryMapper.ToDto(historyRepository);
            }
            throw new NonWipProjectException(projectId);
        }

        public HistoryRepositoryModel UpdateHistoryRepository(string projectId, string historyRepositoryName, HistoryRepositoryModel historyRepositoryUpdateRequest)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                HistoryRepository historyRepositoryUpdate = HistoryRepositoryMapper.FromDto(historyRepositoryUpdateRequest);
                wipProject.UpdateHistoryRepository(historyRepositoryName, historyRepositoryUpdate);
                return HistoryRepositoryMapper.ToDto(historyRepositoryUpdate);
            }
            throw new NonWipProjectException(projectId);
        }

        public void DeleteHistoryRepository(string projectId, string historyRepositoryName)
        {
            if (GetProjectEntity(projectId) is WipProject wipProject)
            {
                wipProject.RemoveHistoryRepository(historyRepositoryName);
                return;
            }
            throw new NonWipProjectException(projectId);
        }
    }
}
