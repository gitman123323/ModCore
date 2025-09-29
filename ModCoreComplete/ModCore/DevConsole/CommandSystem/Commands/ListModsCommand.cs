using System;
using UnityEngine;

public class ListModsCommand : ConsoleCommand
{
    public override string Name => "/ListMods";
    public override string Description => "Lists all loaded MonoBehaviour and IMod mods.";

    public override void Execute(string[] args)
    {
        try
        {
            SafeLogScroll("Loaded MonoBehaviour Mods: ", Color.cyan);
            if (MonoBehaviourLoader.Instance.modGameObjects.Count == 0)
                SafeLogScroll("<color=gray>None</color>");
            else
                foreach (var mod in MonoBehaviourLoader.Instance.modGameObjects)
                    SafeLogScroll($"{mod.GetComponent<MonoBehaviour>().GetType().Name}", Color.green);

           SafeLogScroll("Loaded IMods: ", Color.cyan);
            if (ModLoader.Instance.loadedMods.Count == 0)
                SafeLogScroll("<color=gray>None</color>");
            else
                foreach (var mod in ModLoader.Instance.loadedMods)
                    SafeLogScroll($"{mod.GetType().FullName}", Color.green);
        }
        catch (Exception ex)
        {
            DevConsoleLogScroll.Instance.Log($"{ex.Message}", Color.red);
        }
    }

    private void SafeLogScroll(string message, Color? color = null)
    {
        if (DevConsoleLogScroll.Instance != null)
            DevConsoleLogScroll.Instance.Log(message, color);
        else
            Debug.Log(message);
    }

    private void SafeLogStatic(string message, Color? color = null)
    {
        if (DevConsoleLogStatic.Instance != null)
            DevConsoleLogStatic.Instance.Log(message, color);
        else
            Debug.Log(message);
    }
}
