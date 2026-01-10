using SolutionArchitect.CashFlow.Web.Contracts;
using System.Net;

namespace SolutionArchitect.CashFlow.Web.Services.Clients;

public sealed class CashFlowClient(HttpClient http)
{
    public async Task ExecuteAsync(
        ExecuteCashFlowOperationRequest request,
        CancellationToken cancellationToken)
    {
        var response = await http.PostAsJsonAsync(
            "api/v1/cashflow",
            request,
            cancellationToken);

        if (response.IsSuccessStatusCode)
            return;

        if (response.StatusCode == HttpStatusCode.BadRequest)
            throw new InvalidOperationException("Invalid request data");

        if (response.StatusCode == HttpStatusCode.Conflict)
            throw new InvalidOperationException("Concurrency conflict");

        response.EnsureSuccessStatusCode();
    }
}
