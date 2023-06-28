using ScratchConsole.FluidHttp.Result;
using System.Net.Http.Json;

namespace ScratchConsole.FluidHttp.HttpBuilder;

internal class HttpBuilderWithRequestAndProblemDetails<TRequest, TProblem> : HttpBuilderWithRequest<TRequest>
{
    protected IEnumerable<int> ProblemDetailResponseCodes { get; }
    private Action<Exception>? _handler;

    public HttpBuilderWithRequestAndProblemDetails(IEnumerable<int> responseCode, HttpClient client, HttpRequestMessage request) : base(client, request)
    {
        ProblemDetailResponseCodes = responseCode;
    }

    public HttpBuilderWithRequestAndProblemDetails<TRequest, TProblem> WithExceptionHandler(Action<Exception> handler)
    {
        _handler = handler;
        return this;
    }

    public new async Task<RestUnitResult<TProblem>> SendAsync(CancellationToken token = default)
    {
        HttpResponseMessage? response = null;

        try
        {
            response = await Client.SendAsync(Request, token);

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
        catch (Exception ex) 
        {
            if (_handler != null)
            {
                _handler(ex);
                return RestUnitResult.Failure<TProblem>(response!);
            }

            // no exception handler defined
            throw;
        }
    }
}
