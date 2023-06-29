using ScratchConsole.FluentHttp.Result;

namespace ScratchConsole.FluentHttp.HttpBuilder;

internal class HttpBuilderWithRequest<TRequest> : HttpBuilder
{
    private Action<Exception>? _handler;

    public HttpBuilderWithRequest(HttpClient client, HttpRequestMessage request) : base(client, request)
    {
    }

    public HttpBuilderWithRequestAndProblemDetails<TRequest, TProblemDetails> WithProblemDetails<TProblemDetails>(params int[] responseCodeScope)
        => new(responseCodeScope, Client, Request);

    internal HttpBuilderWithRequest<TRequest> WithExceptionHandler(Action<Exception> handler)
    {
        _handler = handler;
        return this;
    }

    public async virtual Task<RestResult> SendAsync(CancellationToken token = default)
    {
        try
        {
            return await Client.SendAsync(Request, token);
        }
        catch (Exception ex)
        {
            if (_handler is not null)
            {
                _handler(ex);
                return RestResult.Failure();
            }
            throw;
        }
    }
}
