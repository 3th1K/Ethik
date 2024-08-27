namespace Ethik.Utility.Extensions;

public static class TaskExtensions
{
    public static async Task<T?> WhenAnyNotNull<T>(this IEnumerable<Task<T?>> tasks) where T : class
    {
        var taskList = tasks.ToList();
        var taskCompletionSource = new TaskCompletionSource<T?>();

        foreach (var task in taskList)
        {
            _ = task.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully && t.Result != null)
                {
                    taskCompletionSource.TrySetResult(t.Result);
                }
                else if (t.IsFaulted)
                {
                    var exception = t.Exception;
                    taskCompletionSource.TrySetException(exception);

                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        return await await Task.WhenAny(taskCompletionSource.Task, Task.WhenAll(taskList).ContinueWith(_ => (T?)null));
    }
}



