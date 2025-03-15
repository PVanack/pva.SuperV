using pva.SuperV.Model.HistoryRetrieval;

namespace pva.SuperV.Api.Services.History
{
    public interface IHistoryValuesService
    {
        HistoryRawResultModel GetInstanceRawHistoryValues(string projectId, string instanceName, HistoryRequestModel request);
        HistoryResultModel GetInstanceHistoryValues(string projectId, string instanceName, HistoryRequestModel request);
    }
}
