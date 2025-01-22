using pva.SuperV.Engine.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.Engine.HistoryStorage
{
    public class FakeHistoryStorageEngine : IHistoryStorageEngine
    {
        public void DeleteRepository(string projectName, string repositoryName)
        {
        }

        public void HistorizeValues(string? _classTimeSerieId, string instanceName, DateTime dateTime, List<IField> fieldsToHistorize)
        {
        }

        public string UpsertClassTimeSerie<T>(string projectName, string className, HistorizationProcessing<T> historizationProcessing)
        {
            return $"{projectName}_{className}";
        }

        public string? UpsertRepository(string projectName, HistoryRepository repository)
        {
            return $"{projectName}_{repository.Name}";
        }
    }
}
