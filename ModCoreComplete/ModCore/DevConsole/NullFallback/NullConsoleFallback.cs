using UnityEngine;
public class NullConsoleFallback : DevConsoleLogScroll
{
    public void Log(string message, Color color)
    {
        // Do nothing, silently swallow
        Debug.LogWarning("Console is destroyed");
    }
}
