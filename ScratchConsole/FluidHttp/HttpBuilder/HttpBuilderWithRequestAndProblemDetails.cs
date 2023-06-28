using ScratchConsole.FluidHttp.Result;
using System.Net.Http.Json;

namespace ScratchConsole.FluidHttp.HttpBuilder;

internal class HttpBuilderWithRequestAndProblemDetails<TRequest, TProblem> : HttpBuilderWithRequest<TRequest>
{
    protected IEnumerable<int> ProblemDetailResponseCodes { get; }

    public HttpBuilderWithRequestAndProblemDetails(IEnumerable<int> responseCode, HttpClient client, HttpRequestMessage request) : base(client, request)
    {
        ProblemDetailResponseCodes = responseCode;
    }

    public new async Task<RestUnitResult<TProblem>> SendAsync(CancellationToken token = default)
    {
        var response = await Client.SendAsync(Request, token);

        // handle success
        if (response.IsSuccessStatusCode)
        {
            return RestUnitResult.Success<TProblem>(response);
        }

        // handle problem details
        if (ProblemDetailResponseCodes.Contains((int)response.StatusCode))
        {
            var problem = await response.Content.ReadFromJsonAsync<TProblem>(cancellationToken: token);
            return RestUnitResult.Failure(problem!, response);
        }

        return RestUnitResult.Failure<TProblem>(response);
    }
}
