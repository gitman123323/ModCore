=== CommandRegistry ===

Summary:
CommandRegistry manages and executes console commands in a Unity application. It acts as a centralized registry for command registration and input parsing/execution.

Features:

Singleton pattern (CommandRegistry.Instance) for global access.

Maintains a dictionary of commands, indexed by their lowercase names.

Registers new commands through RegisterCommand(ConsoleCommand command).

Parses user input strings and extracts command names and arguments.

Ensures all commands start with a / prefix.

Calls Execute on matching commands.

Logs errors and unknown commands via DevConsoleLogScroll.

Supports fetching all command names for autocomplete or listing.

Usage Notes:

Must be attached to a GameObject in your scene (preferably persistent).

Register custom commands by calling RegisterCommand() at startup or runtime.

Input should begin with a / character to be recognized as a command.

Returns false if the input is invalid or the command is unknown.

You can use GetCommandNames() to fetch a list of registered command strings.

Example:

// Register a command during game initialization
CommandRegistry.Instance.RegisterCommand(new HelpCommand());

// Try to run a command string
CommandRegistry.Instance.TryExecuteCommand("/help general");

Command Input Format:

/commandName arg1 arg2 ...

Command name must follow the slash.

Arguments are space-separated.

Integration Tip:

Pair this with a console UI and DevConsoleLogScroll to allow players or developers to type and run commands at runtime.

Related Components:

ConsoleCommand — Base class or interface for creating custom commands.

DevConsoleLogScroll — UI component for displaying console output.

