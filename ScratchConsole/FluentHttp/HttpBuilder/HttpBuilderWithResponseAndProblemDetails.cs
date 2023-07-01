using ScratchConsole.FluentHttp.Result;
using System.Net.Http.Json;

namespace ScratchConsole.FluentHttp.HttpBuilder;

internal class HttpBuilderWithResponseAndProblemDetails<TResponse, TProblem> : HttpBuilderWithResponse<TResponse>
{
    protected IEnumerable<int> ProblemDetailResponseCodes { get; }
    private Action<Exception>? _handler;


    public HttpBuilderWithResponseAndProblemDetails(IEnumerable<int> responseCodes, HttpClient client, HttpRequestMessage request) : base(client, request)
    {
        ProblemDetailResponseCodes = responseCodes;
    }

    public new HttpBuilderWithResponseAndProblemDetails<TResponse, TProblem> WithExceptionHandler(Action<Exception> handler)
    {
        _handler = handler;
        return this;
    }

    public new async Task<RestResult<TResponse, TProblem>> SendAsync(CancellationToken token = default)
    {
        HttpResponseMessage? response = null;

        try
        {
            response = await Client.SendAsync(Request, token);

            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: token);
                
                return value is not null ?
                    RestResult.Success<TResponse, TProblem>(value, response) :
                    RestResult.Failure<TResponse, TProblem>(default!, response);
            }

            // handle problem details
            if (ProblemDetailResponseCodes.Contains((int)response.StatusCode))
            {
                var problem = await response.Content.ReadFromJsonAsync<TProblem>(cancellationToken: token);
                return RestResult.Failure<TResponse, TProblem>(problem!, response);
            }

            return RestResult.Failure<TResponse, TProblem>(response);
        }
        catch (Exception ex)
        {
            if (_handler is not null)
            {
                _handler(ex);
                return RestResult.Failure<TResponse, TProblem>(response);
            }
            throw;
        }
    }
}