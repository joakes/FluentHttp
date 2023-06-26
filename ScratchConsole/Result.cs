namespace ScratchConsole
{
    public readonly struct Result<TValue, TError>
    {
        private readonly TValue? _value;
        private readonly TError? _error;

        public bool IsSuccess { get; }
        public bool IsError => !IsSuccess;

        private Result(TValue value) 
        {
            IsSuccess = true;
            _value = value;
            _error = default;
        }

        private Result(TError error)
        {
            IsSuccess = false;
            _error = error;
            _value = default;
        }

        public static implicit operator Result<TValue, TError>(TValue value) => new (value);
        public static implicit operator Result<TValue, TError>(TError error) => new (error);
    }
}
