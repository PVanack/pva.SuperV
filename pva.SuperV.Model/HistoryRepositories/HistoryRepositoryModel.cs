using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pva.SuperV.Model.HistoryRepositories
{
    [Description("History repository")]
    public record HistoryRepositoryModel(
        [Description("Name of repository")] string Name)
    {
    }
}
