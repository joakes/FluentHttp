namespace ScratchConsole.FluidHttp.HttpBuilder;

internal class HttpBuilderWithRequest<TRequest> : HttpBuilder
{
    public HttpBuilderWithRequest(HttpClient client, HttpRequestMessage request) : base(client, request)
    {
    }

    public HttpBuilderWithRequestAndProblemDetails<TRequest, TProblemDetails> WithProblemDetails<TProblemDetails>(params int[] responseCodeScope)
        => new(responseCodeScope, Client, Request);
}
