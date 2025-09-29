using System;
using UnityEngine;
using System.IO;

public class ReloadModsCommand : ConsoleCommand
{
    public override string Name => "/ReloadMods";
    public override string Description => "You can reload all mods at once without even needing to restart the game!";

    public override void Execute(string[] args)
    {
        try
        {
            ModCompiler.Instance.CompileMods("All mods were successfully recompiled and reloaded!");
        }
        catch (Exception ex)
        {
            SafeLogScroll($"{ex.Message}", Color.red);
            return;
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