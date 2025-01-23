using pva.SuperV.Engine.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDengine.Driver;

namespace pva.SuperV.Engine.HistoryStorage
{
    public class NullHistoryStorageEngine : IHistoryStorageEngine
    {
        public string UpsertRepository(string projectName, HistoryRepository repository)
        {
            return $"{projectName}_{repository.Name}";
        }

        public void DeleteRepository(string projectName, string repositoryName)
        {
        }

        public string UpsertClassTimeSerie<T>(string repositoryStorageId, string projectName, string className, HistorizationProcessing<T> historizationProcessing)
        {
            return $"{projectName}_{className}";
        }

        public void HistorizeValues(string repositoryStorageId, string classTimeSerieId, string instanceName, DateTime dateTime, List<IField> fieldsToHistorize)
        {
        }

        public List<HistoryRow> GetHistoryValues(string repositoryStorageId, string classTimeSerieId, string instanceName, DateTime from, DateTime to, List<IFieldDefinition> fields)
        {
            return [];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Do nothing
        }
    }
}
