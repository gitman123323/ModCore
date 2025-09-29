=== ModCompiler ===

Summary:
ModCompiler handles dynamic compilation of user mods written in C# scripts.
It compiles mod code into DLL assemblies at runtime, supports foldered mods and loose files,
and triggers the mod loaders to load newly compiled mods automatically.

Features:
- Compiles mod scripts using Roslyn (Microsoft.CodeAnalysis).
- Supports parallel compilation tasks with controlled concurrency.
- Detects changes via file timestamps to skip recompiling unchanged mods.
- Supports both folder-based mods (multiple scripts per mod) and single loose script files.
- Integrates with ModLoader and MonoBehaviourLoader to load compiled mods.
- Logs compilation progress, success, and errors with fallback to Unity console or custom dev consoles.

Lifecycle:
- Singleton pattern: one instance during runtime.
- On Awake, initializes assembly references and concurrency limits.
- On Start, checks for dev consoles and triggers compilation sequence.
- Compilation is asynchronous but called from Unity’s main thread context.
- Upon compilation success, automatically calls mod loaders to refresh loaded mods.

Key Methods:

- CompileMods(string customSuccessMessage = null)
  Entry point to compile all mods. Finds all mods in "UserMods/IMods" and "UserMods/MonoBehaviourMods",
  compiles changed mods into DLLs, and triggers loading of compiled mods.
  Should generally be triggered by the mod compiler system or developer actions.

- CompileModFolderAsync(string dir, string compiledFolder, string modName)
  Compiles all C# files in a mod folder into a single DLL asynchronously.

- CompileLooseFileAsync(string file, string compiledFolder)
  Compiles a single loose C# file into a DLL asynchronously.

- HasModChanged(string path)
  Checks if the mod folder or file has changed since the last compilation based on last write time.

Usage Notes:
- Mod scripts should be placed under "UserMods/IMods" for IMod-style mods or "UserMods/MonoBehaviourMods" for MonoBehaviour mods.
- The compiled DLLs are output in "UserMods/{modType}/Compiled" folders.
- ModCompiler expects DevConsoleLogScroll or DevConsoleLogStatic to be present for logging; otherwise falls back to Unity console.
- Compilation is optimized to skip unchanged mods for efficiency.
- After compilation, ModLoader and MonoBehaviourLoader are called to load fresh mods.
- The compiler uses Roslyn, so it supports modern C# syntax and features.

Logging:
- Compilation progress and errors are logged to dev consoles if available, otherwise to Unity’s Debug.Log.
- Detailed diagnostics on compilation failures are logged for debugging.

Example Workflow:
1. Place or update mod scripts in the appropriate UserMods folder.
2. Trigger `ModCompiler.Instance.CompileMods()`.
3. ModCompiler compiles changed mods into DLLs.
4. ModLoader and MonoBehaviourLoader load the compiled mods.
5. Mods become active in the game without restarting the application.

---