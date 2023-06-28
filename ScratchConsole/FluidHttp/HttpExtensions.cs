using System.Net.Http.Json;
using ScratchConsole.FluidHttp.HttpBuilder;

namespace ScratchConsole.HttpExtensions
{
    internal static class HttpExtensions
    {
        public static HttpBuilderWithResponse<TResponse> Get<TResponse>(this HttpClient client, string url)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, url);
            return new HttpBuilderWithResponse<TResponse>(client, message);
        }

        public static HttpBuilderWithResponse<TResponse> Post<TRequest, TResponse>(this HttpClient client, string url, TRequest data)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(data)
            };
            return new HttpBuilderWithResponse<TResponse>(client, message);
        }

        public static HttpBuilderWithRequest<TRequest> Post<TRequest>(this HttpClient client, string url, TRequest data)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(data)
            };
            return new HttpBuilderWithRequest<TRequest>(client, message);
        }

        // add other HTTP verbs as necessary
    }
}
