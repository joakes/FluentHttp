namespace ScratchConsole.HttpExtensions
{
    public record HttpResult(bool IsSuccess, int StatusCode, string? Message = default, Exception? Exception = default)
    {
        public bool IsFailure => !IsSuccess;        

        public static HttpResult Success(int statusCode) => new(true, statusCode);

        public static HttpResultWithProblemDetails<TProblemDetails> Success<TProblemDetails>(int statusCode) 
            => new(true, statusCode);

        public static HttpResultWithResponse<TResponse> Success<TResponse>(int statusCode, TResponse value)
        {
            ArgumentNullException.ThrowIfNull(value, typeof(TResponse).Name);
            return new(true, statusCode, Value: value);
        }

        public static HttpResultWithResponseProblemDetails<TResponse, TProblemDetails> Success<TResponse, TProblemDetails>(int statusCode, TResponse value)
        {
            ArgumentNullException.ThrowIfNull(value, typeof(TResponse).Name);
            return new(true, statusCode, Value: value);
        }

        public static HttpResult Failure(int statusCode, string message, Exception? ex = default) 
            => new(false, statusCode, message, ex);

        public static HttpResultWithProblemDetails<TProblemDetails> Failure<TProblemDetails>(int statusCode, TProblemDetails? problem = default, string? mesage = default, Exception? ex = default)
            => new(false, statusCode, mesage, problem, ex);

        public static HttpResultWithResponse<TResponse> Failure<TResponse>(int statusCode, string message, Exception? ex = default)
            => new(false, statusCode, message, Exception: ex);

        public static HttpResultWithResponseProblemDetails<TResponse, TProblemDetails> Failure<TResponse, TProblemDetails>(int statusCode, TProblemDetails problem, Exception? ex = default)
            => new(false, statusCode, ProblemDetails: problem, Exception: ex);

        public static HttpResultWithResponseProblemDetails<TResponse, TProblemDetails> Failure<TResponse, TProblemDetails>(int statusCode, string message, Exception? ex = default)
            => new(false, statusCode, Message: message, Exception: ex);
    }

    public record HttpResultWithResponse<TResponse>(bool IsSuccess, int StatusCode, string? Message = default, TResponse? Value = default, Exception? Exception = default)
        : HttpResult(IsSuccess, StatusCode, Message, Exception);

    public record HttpResultWithProblemDetails<TProblemDetails>(bool IsSuccess, int StatusCode, string? Message = default, TProblemDetails? ProblemDetails = default, Exception? Exception = default)
        : HttpResult(IsSuccess, StatusCode, Message, Exception)
    {
        public bool HasProblemDetails => ProblemDetails is not null;

        public static HttpResultWithProblemDetails<TProblemDetails> Failure(int statusCode, TProblemDetails problemDetails, string? message = default, Exception? ex = default)
            => new(false, statusCode, message, problemDetails, Exception: ex);

        public static new HttpResultWithProblemDetails<TProblemDetails> Failure(int statusCode, string message, Exception? ex = default)
            => new(false, statusCode, message, Exception: ex);
    };

    public record HttpResultWithResponseProblemDetails<TResponse, TProblemDetails>(bool IsSuccess, int StatusCode, string? Message = default, TResponse? Value = default, TProblemDetails? ProblemDetails = default, Exception? Exception = default)
        : HttpResultWithResponse<TResponse>(IsSuccess, StatusCode, Message, Value, Exception)
    {
        public bool HasProblemDetails => ProblemDetails is not null;
    };
}
