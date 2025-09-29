using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModLoadErrorDisplayer : MonoBehaviour
{
    private bool hasDisplayedMonoErrors = false;
    private bool hasDisplayedIModErrors = false;

    void Update()
    {
        var errorsMono = MonoBehaviourLoader.Instance?.GetLoadErrors();
        if (!hasDisplayedMonoErrors && errorsMono != null && errorsMono.Count > 0)
        {
            string message = $"MonoBehaviour mods loading errors: {errorsMono.Count}";
            Debug.LogError($"MonoBehaviour mods loading errors: {errorsMono.Count}, Check the Dev console for details.");
            SafeLogScroll(message, Color.red);
            hasDisplayedMonoErrors = true;
        }

        var errorsIMod = ModLoader.Instance?.GetLoadErrors();
        if (!hasDisplayedIModErrors && errorsIMod != null && errorsIMod.Count > 0)
        {
            string message = $"IMod mods loading errors: {errorsIMod.Count}";
            Debug.LogError($"IMod mods loading errors: {errorsIMod.Count}, Check the Dev console for details.");
            SafeLogScroll(message, Color.red);
            hasDisplayedIModErrors = true;
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
