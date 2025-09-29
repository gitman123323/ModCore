=== GUIDispatcher ===

Summary:
GUIDispatcher is a singleton MonoBehaviour that provides a centralized and safe way to register
and invoke custom GUI code using Unity’s OnGUI system. It allows other classes to inject
GUI logic into the rendering phase without needing to subclass MonoBehaviour or manage OnGUI themselves.

Purpose:
- Useful for mods, developer tools, or in-game overlays that need to draw GUI elements.
- Ensures all registered GUI callbacks are executed automatically during Unity's OnGUI phase.

Behavior:
- The class creates a singleton instance at runtime.
- During the OnGUI event, it invokes all registered GUI handlers in the order they were added.

Methods:
- void Register(Action handler)
  Registers a GUI callback. It will be invoked every OnGUI call.

- void Unregister(Action handler)
  Removes a previously registered callback.

- void UnregisterAll()
  Clears all registered GUI handlers.

Lifecycle:
- When a GUIDispatcher is created, it sets itself as the global Instance.
- If another instance already exists, the duplicate is destroyed.

Usage Example:
GUIDispatcher.Instance.Register(() =>
{
    GUI.Label(new Rect(10, 10, 200, 30), "Hello from mod GUI!");
});

Best Practices:
- Only register lightweight GUI code — heavy logic inside OnGUI can impact performance.
- Unregister handlers when no longer needed to avoid redundant calls or memory leaks.
- This system allows modular systems (e.g., mods) to render UI without tightly coupling to Unity’s scene or MonoBehaviour hierarchy.

Notes:
- GUIDispatcher must be present in the scene or created at runtime for GUI callbacks to be invoked.
- You can add it manually to a GameObject or create it via code like:

    GameObject obj = new GameObject("GUIDispatcher");
    obj.AddComponent<GUIDispatcher>();
    DontDestroyOnLoad(obj);

- If you already have a manager that initializes core systems, it's a good place to create it there.
