=== Command Classes Documentation ===

Overview

The ClearConsoleCommand class is a concrete implementation of the abstract ConsoleCommand class. It defines a command for a custom developer console system that, when executed, clears the console's log output.

This class is part of a modular command system where each command is its own class and is registered at runtime. The commands are identified by their unique name and have descriptions to aid the user.

Structure

public class ClearConsoleCommand : ConsoleCommand

This class inherits from ConsoleCommand, meaning it must implement:

Name (string)

Description (string)

Execute(string[] args)

Fields and Properties

public override string Name

Returns the string identifier for this command: "/ClearConsole"

Used to match and invoke the command from the console input.

public override string Description

Returns a short explanation of what the command does: "It clears the DevConsole!"

Method: Execute(string[] args)

This is the entry point when the user invokes the command.

Behavior:

Starts a coroutine named ClearTheMess() which performs the log clearing.

Wrapped in a try/catch block to handle exceptions gracefully and log errors to the console.

Coroutine: ClearTheMess()

Steps:

Calls ResetAndClearConsole() on DevConsoleLogScroll.Instance to clear the UI log.

Waits for 0.3 seconds.

Logs a message "Console Cleared Successfully!" using SafeLogStatic with a light purple color.

This delay allows any UI-related clearing animation or refresh to happen before logging the success message.

Helper Methods

SafeLogScroll(string message, Color? color = null)

Safely logs a message using DevConsoleLogScroll.Instance, falling back to Debug.Log if the instance is null.

SafeLogStatic(string message, Color? color = null)

Same idea as above, but targets DevConsoleLogStatic.Instance.

How to Create a New Command

To create your own command, follow this structure:

public class MyCommand : ConsoleCommand
{
    public override string Name => "/MyCommand";
    public override string Description => "Does something cool.";

    public override void Execute(string[] args)
    {
        // Do something
    }
}

If your command needs to do async or delayed operations, start a coroutine from inside Execute using StartCoroutine.

Best Practices

Always validate arguments inside Execute.

Use SafeLogScroll or SafeLogStatic to prevent null exceptions.

Prefix all commands with / for consistency.

Keep names and descriptions short but descriptive.

Dependencies

DevConsoleLogScroll.Instance

DevConsoleLogStatic.Instance

StartCoroutine() from the static Coroutines utility

Summary

The ClearConsoleCommand is a great example of how to write clean, modular command classes for your custom in-game console. It uses coroutines for timed operations and has fallbacks to prevent errors during logging. You can extend this pattern to add more functionality to your developer tools.