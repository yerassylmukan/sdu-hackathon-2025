namespace WebApplication1.Common;

public class Result
{
    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException();
        if (!isSuccess && string.IsNullOrWhiteSpace(error))
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public string? Error { get; }

    public bool IsFailure => !IsSuccess;

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(string error)
    {
        return new Result(false, error);
    }
}

public class Result<T> : Result
{
    protected internal Result(T value) : base(true, null)
    {
        Value = value;
    }

    protected internal Result(string error) : base(false, error)
    {
        Value = default;
    }

    public T? Value { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public new static Result<T> Failure(string error)
    {
        return new Result<T>(error);
    }
}