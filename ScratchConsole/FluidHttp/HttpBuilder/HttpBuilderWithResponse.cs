﻿using ScratchConsole.FluidHttp.Result;
using System.Net.Http.Json;

namespace ScratchConsole.FluidHttp.HttpBuilder;

internal class HttpBuilderWithResponse<TResponse> : HttpBuilder
{
    private Action<Exception>? _handler;

    public HttpBuilderWithResponse(HttpClient client, HttpRequestMessage request) : base(client, request) { }

    public HttpBuilderWithResponse<TResponse> WithExceptionHandler(Action<Exception> handler)
    {
        _handler = handler;
        return this;
    }

    public new async Task<RestResult<TResponse>> SendAsync(CancellationToken token)
    {
        HttpResponseMessage? response = null;

        try
        {
            response = await Client.SendAsync(Request, token);

            if (response.IsSuccessStatusCode)
            {
                var value = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: token);
                return RestResult.Success(value!, response);
            }

            return RestResult.Failure<TResponse>(response, "Http request retured non-success status code");
        }
        catch (Exception ex)
        {
            if (_handler is not null)
            {
                _handler(ex);
                return RestResult.Failure<TResponse>(response!, "Fatal error during HTTP request processing");
            }
            throw;
        }
    }

    public HttpBuilderWithResponseAndProblemDetails<TResponse, TProblemDetails> WithProblemDetails<TProblemDetails>(params int[] responseCodeScope)
        => new(responseCodeScope, Client, Request);
}
