=== ModConfig ===

Summary:
The ModConfig class provides utility access to the mod base folder used to load mods.
It returns the full file system path to the UserMods folder, adapting automatically
depending on whether the game is running in the Unity Editor or as a built application.

Method:
- GetModBasePath()

Returns:
- A string representing the absolute path to the UserMods directory.

Behavior:
- When running in the Unity Editor, the path points to:
    Assets/UserMods

- When running as a built application, the path points to:
    [ExecutableFolder]/UserMods

Usage Example:
string modPath = ModConfig.GetModBasePath();

Notes:
- The returned path can be used to load mod assemblies, metadata, or any
  file-based assets associated with user-created mods.
- This method ensures that file paths are consistent across development and production environments.
- You are responsible for appending subdirectories like "/Mods" or "/Configs" if needed.