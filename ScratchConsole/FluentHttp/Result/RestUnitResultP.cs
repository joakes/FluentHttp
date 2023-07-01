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
