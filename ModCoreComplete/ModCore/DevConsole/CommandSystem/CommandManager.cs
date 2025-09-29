using UnityEngine;
using System.Collections.Generic;
public class CommandManager : MonoBehaviour
{
    public static CommandManager Instance;
    [Tooltip("List the class names of ConsoleCommands to auto-register.")]
    public List<string> commandClassNames;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        RegisterListedCommands();
    }
    private void RegisterListedCommands()
    {
        foreach (var name in commandClassNames)
        {
            var type = FindCommandTypeByName(name);
            if (type != null)
            {
                ConsoleCommand instance = (ConsoleCommand)System.Activator.CreateInstance(type);
                CommandRegistry.Instance.RegisterCommand(instance);
                Debug.Log($"Registered command: {name}");
            }
            else
            {
                Debug.LogWarning($"Command class '{name}' not found or invalid.");
            }
        }
    }

    private System.Type FindCommandTypeByName(string className)
    {
        // Scan all assemblies
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(ConsoleCommand)) &&
                    !type.IsAbstract && type.Name == className)
                {
                    return type;
                }
            }
        }
        return null;
    }
}
