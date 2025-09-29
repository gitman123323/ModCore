using System;
using System.Collections;
using UnityEngine;
using static Coroutines;

public class ClearConsoleCommand : ConsoleCommand
{
    public override string Name => "/ClearConsole";
    public override string Description => "It clears the DevConsole!";

    public override void Execute(string[] args)
    {
        try
        {
            StartCoroutine(ClearTheMess());
        }
        catch (Exception ex)
        {
            SafeLogScroll($"{ex.Message}", Color.red);
        }
    }

    IEnumerator ClearTheMess()
    {
        DevConsoleLogScroll.Instance.ResetAndClearConsole();
        yield return new WaitForSeconds(0.3f);
        SafeLogStatic("<color=#DDA0DD>Console Cleared Successfully!</color>");
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