using System.Net.Http.Json;

namespace ScratchConsole.HttpExtensions
{
    internal static class HttpExtensions
    {
        public static HttpBuilder MakeGet(this HttpClient client, string url)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, url);
            return new HttpBuilder(client, message);
        }

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
    }

    internal static class RangeExtensions
    {
        public static IEnumerator<int> GetEnumerator(this Range range)
        {
            int start = range.Start.Value;
            int end = range.End.Value;
            if (end > start)
                for (int i = start; i <= end; i++)
                    yield return i;
            else
                for (int i = start; i >= end; i--)
                    yield return i;
        }
    }
}
