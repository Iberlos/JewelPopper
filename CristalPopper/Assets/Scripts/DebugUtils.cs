using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DebugUtils
{
    [Conditional("UNITY_EDITOR")]
    public static void Assert(bool condition, string format, params object[] args)
    {
        if (condition)
        {
            return;
        }

        string message = string.Format(format, args);

        UnityEngine.Debug.LogError(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogError(string format, params object[] args)
    {
        string message = string.Format(format, args);

        UnityEngine.Debug.LogError(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void Log(string format, params object[] args)
    {
        string message = string.Format(format, args);

        UnityEngine.Debug.Log(message);
    }
}
