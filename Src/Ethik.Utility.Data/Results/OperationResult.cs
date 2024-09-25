using Ethik.Utility.Data.Extensions;

namespace Ethik.Utility.Data.Results;

/// <summary>
/// Represents the result of an operation, either successful or failed, with support for stacking errors and metadata.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public sealed class OperationResult<T>
{
    public T? Data { get; }
    public bool IsSuccess { get; }
    public List<OperationError> ErrorStack { get; } = new List<OperationError>();
    public Dictionary<string, object> Metadata { get; } = new Dictionary<string, object>();

    /// <summary>
    /// Private constructor for a successful result.
    /// </summary>
    private OperationResult(T data)
    {
        Data = data;
        IsSuccess = true;
    }

    /// <summary>
    /// Private constructor for a failed result.
    /// </summary>
    private OperationResult(string errorMessage, string errorCode, int depth = 1, Exception? exception = null)
    {
        IsSuccess = false;
        ErrorStack.Add(new OperationError(errorMessage, errorCode, depth, exception));
    }

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    /// <param name="data">The data returned by the operation.</param>
    public static OperationResult<T> Success(T data)
    {
        return new OperationResult<T>(data);
    }

    /// <summary>
    /// Creates a failed operation result using an error message and error code.
    /// </summary>
    public static OperationResult<T> Failure(string errorMessage, string errorCode)
    {
        return new OperationResult<T>(errorMessage, errorCode, 1);
    }

    /// <summary>
    /// Creates a failed operation result using an exception and error code.
    /// </summary>
    public static OperationResult<T> Failure(Exception exception, string errorCode)
    {
        return new OperationResult<T>(exception.Message, errorCode, 1, exception);
    }

    /// <summary>
    /// Creates a failed operation result using an error message, exception, and error code.
    /// </summary>
    public static OperationResult<T> Failure(string errorMessage, Exception exception, string errorCode)
    {
        return new OperationResult<T>(errorMessage, errorCode, 1, exception);
    }

    /// <summary>
    /// Creates a failed operation result with additional metadata.
    /// </summary>
    public static OperationResult<T> Failure(string errorMessage, string errorCode, Dictionary<string, object> metadata)
    {
        var result = new OperationResult<T>(errorMessage, errorCode);
        result.Metadata.AddRange(metadata);
        return result;
    }

    /// <summary>
    /// Creates an operation result from another result, stacking its errors and incrementing their depth.
    /// </summary>
    /// <typeparam name="TInput">The type of the input result data.</typeparam>
    /// <param name="otherResult">The result to stack errors from.</param>
    public static OperationResult<T> From<TInput>(OperationResult<TInput> otherResult)
    {
        var result = new OperationResult<T>(default!);
        result.StackErrors(otherResult);
        result.Metadata.AddRange(otherResult.Metadata);
        return result;
    }

    /// <summary>
    /// Stacks errors from another result and increments their depth.
    /// </summary>
    /// <typeparam name="TInput">The type of the result being stacked from.</typeparam>
    private void StackErrors<TInput>(OperationResult<TInput> otherResult)
    {
        foreach (var error in otherResult.ErrorStack)
        {
            error.IncrementDepth();  // Increment depth for each error being propagated
            ErrorStack.Add(error);
        }
    }

    /// <summary>
    /// Adds a new error with the specified message and increments the depth.
    /// </summary>
    public void AddError(string errorMessage, string errorCode, Exception? exception = null)
    {
        var newError = new OperationError(errorMessage, errorCode, GetCurrentDepth() + 1, exception);
        ErrorStack.Add(newError);
    }

    /// <summary>
    /// Retrieves the current depth based on the last error in the stack.
    /// </summary>
    private int GetCurrentDepth()
    {
        return ErrorStack.Count > 0 ? ErrorStack[^1].Depth : 0;
    }
}