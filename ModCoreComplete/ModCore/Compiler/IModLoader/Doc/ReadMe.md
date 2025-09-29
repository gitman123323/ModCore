=== ModLoader ===

Summary:
ModLoader is responsible for loading, managing, updating, and unloading mods that implement
the IMod interface. It dynamically loads compiled mod assemblies (DLLs) from a specified
folder, resolves dependencies between mods, and handles mod lifecycle events.

Features:
- Loads mod assemblies from a given folder after compilation.
- Detects mods by scanning for classes implementing IMod.
- Reads mod metadata from ModInfoAttribute.
- Resolves dependencies declared via ModDependencyAttribute.
- Updates loaded mods every frame by calling their OnUpdate method.
- Supports automatic unloading of mods that request it.
- Registers mods’ OnGUI handlers with GUIDispatcher for GUI rendering.
- Logs loading progress, errors, and warnings with fallback safe logging.

Lifecycle:
- Singleton pattern: only one ModLoader instance exists at runtime.
- On Awake, sets up the singleton instance or destroys duplicates.
- During Update, calls OnUpdate on each loaded mod and handles unloading.
- On Destroy, unloads all loaded mods gracefully.

Key Methods:

- LoadMods_PostCompile(string compiledFolder, bool calledFromCompiler)
  Loads or reloads mods from the specified folder.  
  ⚠️ Should only be called automatically by the mod compiler after mods are compiled.  
  Scans DLL files, loads mod classes, checks dependencies, creates mod instances,  
  and registers their GUI callbacks.  
  Does not load mods with missing dependencies.

- UnloadMods()  
  Unloads all currently loaded mods by calling their OnUnload methods and unregistering GUI handlers.

- GetLoadErrors()  
  Returns a list of all errors encountered during the last mod loading operation.

Usage Notes:
- Mods must implement the IMod interface to be loaded by this loader.
- Each mod should have a ModInfoAttribute to identify itself. Mods without this attribute are skipped.
- Dependencies declared by ModDependencyAttribute must reference valid mod IDs from other mods' ModInfoAttribute, or the mod will be skipped.
- This loader supports dynamic runtime mod reloading through the mod compiler system.
- All GUI drawing code from mods should be registered via GUIDispatcher for OnGUI rendering.

Logging:
- Errors and important messages are logged both to Unity’s console and to
  internal scrollable and static dev consoles if available.
- Load errors are collected for later retrieval.

Example Scenario:
After mods are compiled into DLLs in a folder, the compiler calls LoadMods_PostCompile
to load and initialize all mods. During the game, ModLoader calls OnUpdate on each mod
and manages unloading when mods request it.

---