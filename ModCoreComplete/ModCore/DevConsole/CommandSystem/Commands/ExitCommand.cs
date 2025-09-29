using System;
using UnityEngine;

public class ExitCommand : ConsoleCommand
{
    public override string Name => "/Exit";
    public override string Description => "Exit the game completely";

    public override void Execute(string[] args)
    {
        try
        {
            Application.Quit();
        }
        catch (Exception ex)
        {
            DevConsoleLogScroll.Instance.Log($"{ex.Message}", Color.red);
        }
    }
}