namespace Ethik.Utility.Data.Results;

/// <summary>
/// Represents an error that occurs during an operation, supporting an error message, exception, depth, and error code.
/// </summary>
public class OperationError
{
    public string ErrorMessage { get; }
    public string ErrorCode { get; }
    public Exception? Exception { get; }
    public int Depth { get; private set; }

    public OperationError(string errorMessage, string errorCode, int depth, Exception? exception = null)
    {
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Depth = depth;
        Exception = exception;
    }

    /// <summary>
    /// Increases the depth of the error as it gets propagated across layers.
    /// </summary>
    public void IncrementDepth() => Depth++;
}