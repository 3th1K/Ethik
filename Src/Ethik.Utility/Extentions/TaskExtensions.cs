namespace Ethik.Utility.Extensions;

/// <summary>
/// Provides extension methods for working with tasks.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Returns the first successfully completed task that is not null from a collection of tasks, or null if no such task exists.
    /// </summary>
    /// <typeparam name="T">The type of the task result.</typeparam>
    /// <param name="tasks">A collection of tasks to monitor.</param>
    /// <returns>
    /// A task that represents the first successfully completed task with a non-null result.
    /// If all tasks complete without a non-null result, returns null.
    /// </returns>
    public static async Task<T?> WhenAnyNotNullAsync<T>(this IEnumerable<Task<T?>> tasks, CancellationToken cancellationToken) where T : class
    {
        // Convert the IEnumerable of tasks into a List for easier manipulation.
        var taskList = tasks.ToList();

        // Create a TaskCompletionSource to manage the result of the first completed task that meets the condition.
        var taskCompletionSource = new TaskCompletionSource<T?>();

        // Iterate through each task in the list.
        foreach (var task in taskList)
        {
            // Attach a continuation to each task.
            _ = task.ContinueWith(t =>
            {
                // If the task completes successfully and its result is not null, set the result.
                if (t.IsCompletedSuccessfully && t.Result != null)
                {
                    taskCompletionSource.TrySetResult(t.Result);
                }
                // If the task is faulted, propagate the exception.
                else if (t.IsFaulted)
                {
                    var exception = t.Exception;
                    taskCompletionSource.TrySetException(exception);
                }
            }, cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        // Wait for either the TaskCompletionSource's task to complete or all tasks to complete with null results.
        return await await Task.WhenAny(
            taskCompletionSource.Task,
            Task.WhenAll(taskList).ContinueWith(_ => (T?)null, TaskScheduler.Default)
        );
    }
}
