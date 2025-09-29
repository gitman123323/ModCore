using System;
using UnityEngine;
public class HelloCommand : ConsoleCommand
{
    public override string Name => "/Hello";
    public override string Description => "Says hello";

    public override void Execute(string[] args)
    {
        try
        {
            SafeLogScroll("<color=#DDA0DD>Hello, Dev!</color>");
        }
        catch (Exception ex)
        {
            SafeLogScroll($"{ex.Message}", Color.red);
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
