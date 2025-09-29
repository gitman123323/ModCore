=== MonoBehaviourLoader ===

Summary:
MonoBehaviourLoader is responsible for loading and managing mods implemented as MonoBehaviour classes.
It dynamically loads compiled mod assemblies (DLLs) from a specified folder, resolves dependencies,
instantiates GameObjects with mod components, and handles unloading.

Features:
- Loads mods from DLLs where classes derive from MonoBehaviour.
- Reads mod metadata from ModInfoAttribute.
- Resolves dependencies declared via ModDependencyAttribute.
- Instantiates a separate GameObject per mod, adds the mod component, and marks it DontDestroyOnLoad.
- Supports mod unloading by destroying mod GameObjects.
- Logs loading progress, errors, and warnings with fallback safe logging.

Lifecycle:
- Singleton pattern ensures only one instance exists.
- On Awake, sets up singleton or destroys duplicates.
- On Destroy, unloads all loaded mods.

Key Methods:

- LoadMods_PostCompile(string compiledFolder, bool calledFromCompiler)
  Loads or reloads mods from the specified folder.  
  ⚠️ Intended to be called automatically by the mod compiler after compilation.  
  Scans DLL files, loads mod classes, checks dependencies, and instantiates mod GameObjects.

- UnloadMods()
  Unloads all loaded mods by destroying their GameObjects and clearing internal state.

- GetLoadErrors()
  Returns a list of all errors encountered during the last mod loading operation.

Usage Notes:
- Mods must be MonoBehaviour subclasses and have a ModInfoAttribute to be loaded.
- Dependencies declared with ModDependencyAttribute must reference valid mod IDs from other mods.
- Avoid calling LoadMods_PostCompile manually unless you are certain of the context.
- This loader complements the IMod-based ModLoader, allowing two different mod styles.

Logging:
- Uses DevConsoleLogScroll and DevConsoleLogStatic if available, falling back to Unity’s console.
- Collects load errors internally for later retrieval.

Example Scenario:
After mod compilation, the compiler invokes LoadMods_PostCompile to instantiate mod GameObjects
with their MonoBehaviour mod scripts. Mods are kept alive across scenes using DontDestroyOnLoad.

---