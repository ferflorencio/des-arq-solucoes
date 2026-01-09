using System;
using System.Collections.Generic;
using System.Text;

namespace SolutionArchitect.CashFlow.FinancialConsolidate.Worker.Data.Cache;

public sealed record RedisOptions
{
    public string ConnectionString { get; set; } = default!;
}
