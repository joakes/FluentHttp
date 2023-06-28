using ScratchConsole.FluidHttp.Result;
using System.Net.Http.Json;

namespace ScratchConsole.FluidHttp.HttpBuilder;

internal class HttpBuilderWithResponseAndProblemDetails<TResponse, TProblem> : HttpBuilderWithResponse<TResponse>
{
    protected IEnumerable<int> ProblemDetailResponseCodes { get; }

    public HttpBuilderWithResponseAndProblemDetails(IEnumerable<int> responseCodes, HttpClient client, HttpRequestMessage request) : base(client, request)
    {
        ProblemDetailResponseCodes = responseCodes;
    }

    public new async Task<RestResult<TResponse, TProblem>> SendAsync(CancellationToken token = default)
    {
        var response = await Client.SendAsync(Request, token);

        if (response.IsSuccessStatusCode)
        {
            var value = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: token);
            return RestResult.Success<TResponse, TProblem>(value!, response);
        }

        // handle problem details
        if (ProblemDetailResponseCodes.Contains((int)response.StatusCode))
        {
            var problem = await response.Content.ReadFromJsonAsync<TProblem>(cancellationToken: token);
            return RestResult.Failure<TResponse, TProblem>(problem!, response);
        }

        return RestResult.Failure<TResponse, TProblem>(response);
    }
}