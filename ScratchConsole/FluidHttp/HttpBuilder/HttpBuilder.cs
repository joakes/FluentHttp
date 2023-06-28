using ScratchConsole.FluidHttp.Result;

namespace ScratchConsole.FluidHttp.HttpBuilder;

internal abstract class HttpBuilder
{
    protected HttpClient Client { get; }
    protected HttpRequestMessage Request { get; }

    public HttpBuilder(HttpClient client, HttpRequestMessage request)
    {
        Client = client;
        Request = request;
    } 
}
