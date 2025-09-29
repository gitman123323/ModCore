=== Null Console Fallback Documentation ===

Overview

The NullConsoleFallback and NullConsoleFallbackStatic classes are fallback implementations for the DevConsole logging system in Unity. They are used when the actual logging components (DevConsoleLogScroll and DevConsoleLogStatic) are destroyed or unavailable.

Instead of throwing null reference exceptions when attempting to log to the console after it has been destroyed, these classes gracefully handle such situations by logging a warning in the Unity Console.

Purpose

Unity components can be destroyed during runtime (e.g., scene unloads, errors, manual destruction). If the console instance is destroyed but other parts of the game still attempt to log messages, a NullConsoleFallback class can prevent crashes and maintain debug output through Unity’s default logging system.

NullConsoleFallback

using UnityEngine;

public class NullConsoleFallback : DevConsoleLogScroll
{
    public void Log(string message, Color color)
    {
        // Do nothing, silently swallow
        Debug.LogWarning("Console is destroyed");
    }
}

Usage

Used as a runtime replacement when DevConsoleLogScroll.Instance is null or has been destroyed:

if (DevConsoleLogScroll.Instance == null)
    DevConsoleLogScroll.Instance = new NullConsoleFallback();

NullConsoleFallbackStatic

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

Usage

Used as a fallback when DevConsoleLogStatic.Instance is unavailable:

if (DevConsoleLogStatic.Instance == null)
    DevConsoleLogStatic.Instance = new NullConsoleFallbackStatic();

Benefits

Prevents null reference exceptions from uninitialized or destroyed console instances.

Logs a warning to Unity's Debug console instead of breaking the game flow.

Safe fallback during scene transitions or dynamic runtime environments.

Limitations

Logging is redirected only to Debug.LogWarning, not to the in-game DevConsole.

Color formatting is not visible in the Unity Console.

Summary

NullConsoleFallback and NullConsoleFallbackStatic provide safe and lightweight fallbacks to prevent console-related issues when the main DevConsole UI is not available. They ensure continued visibility of logs while avoiding errors that could disrupt gameplay or development workflows.