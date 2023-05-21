namespace Scopely.Core.Result;

public class Result<T> : Result
{
    public T? Value { get; private set; }

    public static Result<T> Ok(T value)
        => new()
        {
            IsOk = true,
            Value = value
        };

    public new static Result<T> Error(string errorMessage)
        => new()
        {
            Message = errorMessage
        };

    public new static Result<T> Unexpected(Exception exception)
        => new()
        {
            Exception = exception,
            Message = exception.Message
        };
}

public class Result
{
    public bool IsOk { get; protected set; }
    public string? Message { get; protected set; }
    public Exception? Exception { get; protected set; }
    public bool IsException => Exception != null;

    public static Result Ok()
        => new()
        {
            IsOk = true
        };

    public static Result Error(string errorMessage)
        => new()
        {
            Message = errorMessage
        };

    public static Result Unexpected(Exception exception)
        => new()
        {
            Exception = exception,
            Message = exception.Message
        };
}