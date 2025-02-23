using pva.SuperV.Engine.HistoryStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.Model.HistoryRepositories
{
    public static class HistoryRepositoryMapper
    {
        public static HistoryRepositoryModel ToDto(HistoryRepository repository)
            => new(repository.Name);
    }
}
