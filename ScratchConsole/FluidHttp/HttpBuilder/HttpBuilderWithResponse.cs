using ScratchConsole.FluidHttp.Result;
using System.Net.Http.Json;

namespace ScratchConsole.FluidHttp.HttpBuilder;

internal class HttpBuilderWithResponse<TResponse> : HttpBuilder
{
    public HttpBuilderWithResponse(HttpClient client, HttpRequestMessage request) : base(client, request) { }

    public new async Task<RestResult<TResponse>> SendAsync(CancellationToken token)
    {
        var response = await Client.SendAsync(Request, token);

        if (response.IsSuccessStatusCode)
        {
            var value = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: token);
            return RestResult.Success(value!, response);
        }

        return RestResult.Failure<TResponse>(response, "Http request retured non-success status code");
    }

    public HttpBuilderWithResponseAndProblemDetails<TResponse, TProblemDetails> WithProblemDetails<TProblemDetails>(params int[] responseCodeScope)
        => new(responseCodeScope, Client, Request);
}
