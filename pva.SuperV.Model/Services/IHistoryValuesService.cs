using pva.SuperV.Model.HistoryRetrieval;

namespace pva.SuperV.Model.Services
{
    public interface IHistoryValuesService
    {
        Task<HistoryRawResultModel> GetInstanceRawHistoryValuesAsync(string projectId, string instanceName, HistoryRequestModel request);
        Task<HistoryResultModel> GetInstanceHistoryValuesAsync(string projectId, string instanceName, HistoryRequestModel request);
        Task<HistoryStatisticsRawResultModel> GetInstanceRawHistoryStatisticsAsync(string projectId, string instanceName, HistoryStatisticsRequestModel request);
        Task<HistoryStatisticsResultModel> GetInstanceHistoryStatisticsAsync(string projectId, string instanceName, HistoryStatisticsRequestModel request);
    }
}
