=== CommandManager ===

Summary:
The CommandManager is responsible for automatically registering console commands at runtime.
It reads a list of command class names (derived from `ConsoleCommand`) and registers their instances into the `CommandRegistry`.

Features:
- Singleton pattern for global access.
- Holds a list of command class names to auto-register on Start.
- Dynamically searches all loaded assemblies to find and instantiate commands by their class names.
- Logs registration success and warnings for missing or invalid command classes.

Usage:
1. Populate the `commandClassNames` list (in the inspector or via code) with the names of your command classes that inherit from `ConsoleCommand`.
2. On game start, the manager will create instances of these commands and register them automatically.
3. Ensure your command classes are not abstract and inherit from `ConsoleCommand`.

Example:
```csharp
public class MyCommand : ConsoleCommand
{
    // Implement command specifics here
}

Add "MyCommand" to the commandClassNames list to have it auto-registered.

Notes:

This manager depends on the CommandRegistry singleton to handle the actual command registration.

Reflection is used to find command classes dynamically, so command class names must be spelled exactly.

Duplicate registrations are prevented by design of the CommandRegistry.