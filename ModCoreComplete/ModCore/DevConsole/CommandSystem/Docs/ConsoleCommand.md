=== ConsoleCommand ===

Summary:
The ConsoleCommand class is an abstract base class representing a custom console command that can be registered and executed by the CommandRegistry. All custom commands must inherit from this class and implement the required members.

Features:

Abstract class meant to be extended for defining custom console commands.

Requires implementing:

Name: Unique identifier for the command (without the / prefix).

Description: Short description explaining the purpose of the command.

Execute(string[] args): Method that executes the command logic using provided arguments.

Usage Notes:

Inherit from ConsoleCommand to create new commands.

Register the derived command using CommandRegistry.Instance.RegisterCommand(...).

Ensure command names are lowercase and unique within the registry.

Commands should validate and parse their arguments appropriately inside Execute.

Example:

public class HelloWorldCommand : ConsoleCommand
{
    public override string Name => "/hello";
    public override string Description => "Prints a hello message to the console.";

    public override void Execute(string[] args)
    {
        DevConsoleLogScroll.Instance.Log("Hello, world!", Color.green);
    }
}

Related Components:

CommandRegistry: Registers and manages all ConsoleCommand instances.

DevConsoleLogScroll: Used for logging output to a custom console UI.

