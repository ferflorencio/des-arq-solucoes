using SolutionArchitect.CashFlow.Web.Contracts;

namespace SolutionArchitect.CashFlow.Web.Services.Clients;

public sealed class CashFlowConsolidateClient(HttpClient http)
{
    public async Task<ConsolidatedCashFlowResponse?> GetByDateAsync(DateTime date, CancellationToken cancellationToken)
    {
        var url = $"api/v1/cashflow/consolidated/{date.Year}/{date.Month}/{date.Day}";
        var response = await http.GetAsync(url, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ConsolidatedCashFlowResponse>(cancellationToken);
    }
}
