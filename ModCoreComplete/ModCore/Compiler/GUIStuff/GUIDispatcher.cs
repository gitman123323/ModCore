using UnityEngine;
using System;
using System.Collections.Generic;

public class GUIDispatcher : MonoBehaviour
{
    public static GUIDispatcher Instance { get; private set; }

    private readonly List<Action> guiHandlers = new();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnGUI()
    {
        foreach (var handler in guiHandlers)
        {
            handler?.Invoke();
        }
    }

    public void Register(Action handler)
    {
        if (!guiHandlers.Contains(handler))
            guiHandlers.Add(handler);
    }

    public void Unregister(Action handler)
    {
        guiHandlers.Remove(handler);
    }

    public void UnregisterAll()
    {
        guiHandlers.Clear();
    }
}
