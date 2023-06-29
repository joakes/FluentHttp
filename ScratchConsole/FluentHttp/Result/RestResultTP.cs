namespace ScratchConsole.FluentHttp.Result;

internal readonly struct RestResult<T, P> : IRestResult<T, P>
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    private readonly T _value;
    public T Value => IsSuccess ? _value : throw new Exception(); // TODO subtype exception

    private readonly P _problem;
    public P Problem => IsFailure ? _problem : throw new Exception(); // TODO subtype exception

    public HttpResponseMessage ResponseMessage { get; }

    public RestResult(bool isSuccess, T value, P problem, HttpResponseMessage responseMessage)
    {
        IsSuccess = isSuccess;
        _value = value;
        _problem = problem;
        ResponseMessage = responseMessage;
    }
}

