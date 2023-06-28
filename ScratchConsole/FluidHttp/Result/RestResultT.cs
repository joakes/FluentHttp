﻿namespace ScratchConsole.FluidHttp.Result;

internal readonly struct RestResult<T> : IRestResult<T>
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    private readonly T _value;
    public T Value => IsSuccess ? _value : throw new Exception(); // TODO subtype exception

    private readonly string _error;
    public string Problem => IsFailure ? _error : string.Empty;

    public HttpResponseMessage ResponseMessage { get; }

    public RestResult(bool isSuccess, string error, T value, HttpResponseMessage responseMessage)
    {        
        _error = error;
        _value = value;
        IsSuccess = isSuccess;
        ResponseMessage = responseMessage;
    }    
}
