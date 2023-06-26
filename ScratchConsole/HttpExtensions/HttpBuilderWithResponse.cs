using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace ScratchConsole.HttpExtensions
{
    internal class HttpBuilder
    {
        protected HttpClient Client { get; }
        protected HttpRequestMessage Request { get; }

        public HttpBuilder(HttpClient client, HttpRequestMessage request)
        {
            Client = client;
            Request = request;
        }

        public async virtual Task<HttpResult> SendAsync(ILogger logger, CancellationToken token = default)
        {
            var response = await Client.SendAsync(Request, token);

            var statusCode = (int)response.StatusCode;

            return response.IsSuccessStatusCode ?
                HttpResult.Success(statusCode) :
                HttpResult.Failure(statusCode, "Request failed", default);
        }

        protected async Task<(bool, HttpResponseMessage?, Exception?)> TrySend(ILogger logger, CancellationToken token = default)
        {
            try
            {
                var response = await Client.SendAsync(Request, token);
                return (true, response, null);
            }
            catch (HttpRequestException ex) 
            {
                return (false, null, ex);
            }
            catch (Exception ex)
            {
                return (false, null, ex);
            }
        }

        protected static async Task<(bool, T?, Exception?)> TryReadFromJson<T>(HttpContent content, ILogger logger, CancellationToken token = default)
        {
            try
            {
                var value = await content.ReadFromJsonAsync<T>(cancellationToken: token);
                return (value is not null, value, default);
            }
            catch (Exception ex)
            {
                return (false, default, ex);
            }
        }
    }

    internal class HttpBuilderWithRequest<TRequest> : HttpBuilder
    {
        public HttpBuilderWithRequest(HttpClient client, HttpRequestMessage request) : base(client, request)
        {
        }

        public HttpBuilderWithRequestAndProblemDetails<TRequest, TProblemDetails> WithProblemDetails<TProblemDetails>(params int[] responseCodeScope)
            => new(responseCodeScope, Client, Request);
    }

    internal class HttpBuilderWithResponse<TResponse> : HttpBuilder
    {
        public HttpBuilderWithResponse(HttpClient client, HttpRequestMessage request) : base(client, request)
        {
        }

        public HttpBuilderWithResponseAndProblemDetails<TResponse, TProblemDetails> WithProblemDetails<TProblemDetails>(params int[] responseCodeScope) 
            => new(responseCodeScope, Client, Request);
    }

    internal class HttpBuilderWithRequestAndProblemDetails<TRequest, TProblemDetails> : HttpBuilderWithRequest<TRequest>
    {
        protected IEnumerable<int> ProblemDetailResponseCodes { get; }

        public HttpBuilderWithRequestAndProblemDetails(IEnumerable<int> responseCode, HttpClient client, HttpRequestMessage request) : base(client, request)
        {
            ProblemDetailResponseCodes = responseCode;
        }

        public new async Task<HttpResultWithProblemDetails<TProblemDetails>> SendAsync(ILogger logger, CancellationToken token = default)
        {
            var (responseReturned, response, exception) = await TrySend(logger, token);

            if (responseReturned == false)
                return HttpResultWithProblemDetails<TProblemDetails>.Failure(0, "Error sending request", exception);

            var statusCode = (int)response!.StatusCode;

            // handle success
            if (response.IsSuccessStatusCode)
            {
                return HttpResult.Success<TProblemDetails>(statusCode);
            }

            // handle problem details
            if (ProblemDetailResponseCodes.Contains(statusCode))
            {
                (_, TProblemDetails? value, Exception? ex) = await TryReadFromJson<TProblemDetails>(response.Content, logger, token);
                return HttpResultWithProblemDetails<TProblemDetails>.Failure(statusCode, value!, ex: ex);
            }

            return HttpResultWithProblemDetails<TProblemDetails>.Failure(statusCode, "Unhandled response code");
        }
    }

    internal class HttpBuilderWithResponseAndProblemDetails<TResponse, TProblemDetails> : HttpBuilderWithResponse<TResponse>
    {
        protected IEnumerable<int> ProblemDetailResponseCodes { get; }

        public HttpBuilderWithResponseAndProblemDetails(IEnumerable<int> responseCodes, HttpClient client, HttpRequestMessage request) : base(client, request)
        {
            ProblemDetailResponseCodes = responseCodes;
        }

        public new async Task<HttpResultWithResponseProblemDetails<TResponse, TProblemDetails>> SendAsync(ILogger logger, CancellationToken token = default)
        {
            var (responseReturned, response, exception) = await TrySend(logger, token);

            if (responseReturned == false)
                return HttpResult.Failure<TResponse, TProblemDetails>(0, "Error sending request", exception);

            var statusCode = (int)response!.StatusCode;

            // handle success
            if (response.IsSuccessStatusCode)
            {
                (bool jsonWasRead, TResponse? value, Exception? ex)  = await TryReadFromJson<TResponse>(response.Content, logger, token);

                return jsonWasRead ?
                    HttpResult.Success<TResponse, TProblemDetails>(statusCode, value!) :
                    HttpResult.Failure<TResponse, TProblemDetails>(statusCode, "Body serialization failure", ex);
            }

            // handle problem details
            if (ProblemDetailResponseCodes.Contains(statusCode))
            {
                (_, TProblemDetails? value, Exception? ex) = await TryReadFromJson<TProblemDetails>(response.Content, logger, token);
                return HttpResult.Failure<TResponse, TProblemDetails>(statusCode, value!, ex);
            }

            return HttpResult.Failure<TResponse, TProblemDetails>(statusCode, "Unhandled response code");
        }
    }
}
