using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullConsoleFallbackStatic : DevConsoleLogStatic
{
    public void Log(string message, Color color)
    {
        // Do nothing, silently swallow
        Debug.LogWarning("Console is destroyed");
    }
}
