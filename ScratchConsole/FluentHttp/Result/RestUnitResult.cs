namespace ScratchConsole.FluentHttp.Result;

public static class RestUnitResult
{
    internal static RestUnitResult<P> Success<P>(HttpResponseMessage responseMessage) => new(true, default!, responseMessage);
    internal static RestUnitResult<P> Failure<P>(P problem, HttpResponseMessage response) => new(false, problem, response);
    internal static RestUnitResult<P> Failure<P>(HttpResponseMessage response) => new(false, default!, response);
}