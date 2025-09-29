IMod Interface
Summary:
IMod defines the core lifecycle methods that all mods must implement to work with the ModLoader system.
It establishes the basic contract for loading, updating, unloading, and optional Unity-like update behaviors such as fixed and late updates, as well as GUI rendering.

Interface Methods:
void OnLoad()
Called once when the mod is first loaded. Use this to initialize resources or set up state.

void OnUnload()
Called when the mod is being unloaded. Use this to clean up resources and unregister callbacks.

void OnUpdate()
Called once per frame. Use this for logic that should run every rendered frame.

void OnFixedUpdate()
(Optional) Called at a fixed timestep, ideal for physics-related logic.

void OnLateUpdate()
(Optional) Called after all Update() calls. Useful for post-processing logic.

bool ShouldUnload()
Called each update cycle to determine whether the mod wants to be unloaded.
Return true to signal the ModLoader to unload this mod.
Should return false by default unless unloading is required.

void OnGUI()
Called every GUI frame if the mod is registered with GUIDispatcher.
Use this to draw any mod-specific GUI elements.

Usage Notes:
All mods must implement this interface to be compatible with the ModLoader.

Optional methods like OnFixedUpdate() and OnLateUpdate() are only called if defined in the mod class.

The interface provides default (empty) method bodies, so mods may override only the ones they need.

Be careful to avoid recursion in ShouldUnload(). Do not return ShouldUnload(); inside itself.

Example Implementation:

```csharp
public class MyMod : IMod
{
    public void OnLoad() 
    {
        // Initialization logic here
    }

    public void OnUnload() 
    {
        // Cleanup logic here
    }

    public void OnUpdate() 
    {
        // Per-frame logic
    }

    public void OnFixedUpdate() 
    {
        // Fixed-timestep logic
    }

    public void OnLateUpdate() 
    {
        // Post-frame logic
    }

    public bool ShouldUnload() 
    {
        return false; // Change to true or use a bool to request unload
    }

    public void OnGUI() 
    {
        // GUI rendering logic
    }
}