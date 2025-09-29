using System.Collections.Generic;
using UnityEngine;

public class CommandRegistry : MonoBehaviour
{
    public static CommandRegistry Instance;

    private Dictionary<string, ConsoleCommand> commands = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterCommand(ConsoleCommand command)
    {
        commands[command.Name.ToLower()] = command;
    }

    public bool TryExecuteCommand(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        string[] parts = input.Trim().Split(' ');
        string commandName = parts[0];
        string[] args = parts.Length > 1 ? input.Substring(commandName.Length + 1).Split(' ') : new string[0];

        if (!commandName.StartsWith("/"))
        {
            DevConsoleLogScroll.Instance.Log("Commands must start with a '/' character. Try again.", Color.red);
            return false;
        }

        commandName = commandName.ToLower();

        if (commands.TryGetValue(commandName, out var command))
        {
            command.Execute(args);
            return true;
        }

        DevConsoleLogScroll.Instance.Log($"Unknown command: {commandName}", Color.red);
        return false;
    }

    public List<string> GetCommandNames()
    {
        return new List<string>(commands.Keys);
    }
}
