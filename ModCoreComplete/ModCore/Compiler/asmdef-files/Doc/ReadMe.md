# asmdef-files

This folder contains Assembly Definition files (`.asmdef`) used to separate parts of the codebase into isolated Unity assemblies.

- `ModCompiler.asmdef`: Defines the assembly for the Mod Compiler system.  
  It's used so that scripts can reference each other cleanly without circular dependencies, and to allow faster compilation.

⚠️ **Warning**: Removing or modifying this file might break references between systems (e.g., if `MonoBehaviour` mods or core tools depend on this compiler assembly).  
Not 100% sure what breaks if you delete it — but it's definitely important for maintaining clean references across the modding system.