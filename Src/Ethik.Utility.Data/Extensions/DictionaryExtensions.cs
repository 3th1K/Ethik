namespace Ethik.Utility.Data.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Adds the elements of the specified dictionary to the current dictionary.
    /// </summary>
    public static void AddRange(this Dictionary<string, object> target, Dictionary<string, object>? source)
    {
        if (source == null) return;
        foreach (var pair in source)
        {
            target[pair.Key] = pair.Value;
        }
    }
}