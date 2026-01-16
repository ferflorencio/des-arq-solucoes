using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SolutionArchitect.CashFlow.Api.Application.Config
{
    public interface IResiliencePipelineExecutor
    {
        Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken);
    }
}
