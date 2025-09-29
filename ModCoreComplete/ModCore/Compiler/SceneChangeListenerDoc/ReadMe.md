=== SceneChangeListener ===

Summary:
SceneChangeListener listens for Unity scene load and active scene change events.
It tracks the current active scene and triggers a one-time action whenever the active scene changes after the initial load.

Features:
- Singleton pattern for easy global access.
- Subscribes to SceneManager events: sceneLoaded and activeSceneChanged.
- Logs scene load and scene switch events to Unity console.
- Ignores the initial active scene change callback fired on startup.
- Provides a hook (`TriggerOnceOnSceneChange`) to run code once per scene change.
- Sends a message to DevConsoleLogScroll on scene change.

Usage Notes:
- Attach this script to a persistent GameObject to monitor scene changes throughout the game.
- The first active scene change event after startup is ignored to prevent false triggers.
- Modify `TriggerOnceOnSceneChange` to add custom logic on scene switches.

Example:
```csharp
private void TriggerOnceOnSceneChange()
{
    // Custom code that runs once when the active scene changes
    Debug.Log($"Scene switched to {currentScene.name}");
}