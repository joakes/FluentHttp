using ScratchConsole.FluidHttp.Result;

namespace ScratchConsole.FluidHttp.HttpBuilder;

internal class HttpBuilder
{
    protected HttpClient Client { get; }
    protected HttpRequestMessage Request { get; }

    public HttpBuilder(HttpClient client, HttpRequestMessage request)
    {
        Client = client;
        Request = request;
    }

    public async virtual Task<RestResult> SendAsync(CancellationToken token = default) => await Client.SendAsync(Request, token);
}
