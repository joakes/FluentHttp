namespace ScratchConsole.FluentHttp.Result;

internal readonly struct RestResult : IRestResult
{
    public HttpResponseMessage ResponseMessage { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    internal RestResult(HttpResponseMessage? response)
    {
        IsSuccess = response?.IsSuccessStatusCode ?? false;
        ResponseMessage = response!;
    }

    public static implicit operator RestResult(HttpResponseMessage? response) => new(response);

    internal static RestResult Failure() => new(null);

    // RestResult<T>
    internal static RestResult<T> Success<T>(T value, HttpResponseMessage response) => new(true, string.Empty, value, response);
    internal static RestResult<T> Failure<T>(HttpResponseMessage response, string error) => new(false, error, default!, response);

    // RestResult<T,P>
    internal static RestResult<T, P> Success<T, P>(T value, HttpResponseMessage response) => new(true, value, default!, response);
    internal static RestResult<T, P> Failure<T, P>(P problem, HttpResponseMessage response) => new(false, default!, problem, response);
    internal static RestResult<T, P> Failure<T, P>(HttpResponseMessage response) => new(false, default!, default!, response);
}
