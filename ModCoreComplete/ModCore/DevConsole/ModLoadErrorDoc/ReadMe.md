
# ModLoadErrorDisplayer

## Summary
`ModLoadErrorDisplayer` is a Unity MonoBehaviour component responsible for monitoring and displaying mod loading errors from two mod loaders: `MonoBehaviourLoader` and `ModLoader`. It checks for errors during mod loading and reports them once through Unity's debug console and the in-game developer console.

## Features
- Tracks mod loading errors separately for MonoBehaviour mods and IMod mods.
- Displays error counts only once to avoid repeated log spam.
- Uses the `DevConsoleLogScroll` component to show error messages in the in-game console.
- Falls back to Unity’s default `Debug.Log` if the developer console is not available.

## Usage
- Attach this component to a persistent GameObject in your scene.
- It will automatically check every frame for new loading errors reported by the mod loaders.
- On detecting errors for the first time, it logs a warning to the Unity console and a colored message to the developer console.

## How it works
- The `Update()` method polls the error lists from both mod loaders.
- If errors exist and haven't been logged yet, it logs the error count and marks them as displayed.
- The logging is done safely through `SafeLogScroll` to avoid null references if the developer console is missing.
- Contains a fallback logging method `SafeLogStatic` (not currently used in Update).

## Code Example

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModLoadErrorDisplayer : MonoBehaviour
{
    private bool hasDisplayedMonoErrors = false;
    private bool hasDisplayedIModErrors = false;

    void Update()
    {
        var errorsMono = MonoBehaviourLoader.Instance?.GetLoadErrors();
        if (!hasDisplayedMonoErrors && errorsMono != null && errorsMono.Count > 0)
        {
            string message = $"MonoBehaviour mods loading errors: {errorsMono.Count}";
            Debug.LogError($"MonoBehaviour mods loading errors: {errorsMono.Count}, Check the Dev console for details.");
            SafeLogScroll(message, Color.red);
            hasDisplayedMonoErrors = true;
        }

        var errorsIMod = ModLoader.Instance?.GetLoadErrors();
        if (!hasDisplayedIModErrors && errorsIMod != null && errorsIMod.Count > 0)
        {
            string message = $"IMod mods loading errors: {errorsIMod.Count}";
            Debug.LogError($"IMod mods loading errors: {errorsIMod.Count}, Check the Dev console for details.");
            SafeLogScroll(message, Color.red);
            hasDisplayedIModErrors = true;
        }
    }

    private void SafeLogScroll(string message, Color? color = null)
    {
        if (DevConsoleLogScroll.Instance != null)
            DevConsoleLogScroll.Instance.Log(message, color);
        else
            Debug.Log(message);
    }

    private void SafeLogStatic(string message, Color? color = null)
    {
        if (DevConsoleLogStatic.Instance != null)
            DevConsoleLogStatic.Instance.Log(message, color);
        else
            Debug.Log(message);
    }
}
```