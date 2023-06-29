namespace ScratchConsole.FluentHttp.Result;

internal readonly struct RestUnitResult<P> : IRestUnitResult<P>
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public HttpResponseMessage ResponseMessage { get; }

    private readonly P _problem;
    public P Problem => IsFailure ? _problem : throw new Exception(); // TODO subtype exception

    internal RestUnitResult(bool isSuccess, P problem, HttpResponseMessage responseMessage)
    {
        IsSuccess = isSuccess;
        _problem = problem;
        ResponseMessage = responseMessage;
    }
}

public static class RestUnitResult
{
    internal static RestUnitResult<P> Success<P>(HttpResponseMessage responseMessage) => new(true, default!, responseMessage);
    internal static RestUnitResult<P> Failure<P>(P problem, HttpResponseMessage response) => new(false, problem, response);
    internal static RestUnitResult<P> Failure<P>(HttpResponseMessage response) => new(false, default!, response);
}