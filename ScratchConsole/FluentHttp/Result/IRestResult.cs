namespace ScratchConsole.FluentHttp.Result;
public interface IRestResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    HttpResponseMessage ResponseMessage { get; }
}

public interface IRestValue<out T>
{
    T Value { get; }
}

public interface IRestProblem<out P>
{
    P Problem { get; }
}

public interface IRestResult<out T, out P> : IRestValue<T>, IRestUnitResult<P> { }

public interface IRestResult<out T> : IRestResult<T, string> { }

public interface IRestUnitResult<out P> : IRestResult, IRestProblem<P> { }
