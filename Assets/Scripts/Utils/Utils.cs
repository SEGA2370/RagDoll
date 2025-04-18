using UnityEngine;

public static class Utils
{
    public static void DebugLog(string message)
    {
        Debug.Log($"{Time.time} {message}");
    }

    public static void DebugLogWarning(string message)
    {
        Debug.Log($"{Time.time} {message}");
    }

    public static void DebugLogError(string message)
    {
        Debug.Log($"{Time.time} {message}");
    }
}