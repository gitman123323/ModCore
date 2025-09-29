using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ModInfo("TestMod", "NullCore-Systems", "1.0.0")]
public class TestMod : MonoBehaviour
{
    void Start()
    {
        try
        {
            DevConsoleLogScroll.Instance.Log($"{"TestMod says"}: Yes you saw it, i am indeed a MonoBehaviour Mod!", Color.cyan);
            //Destroy(ConsoleUIHook.Instance.gameObject);
        }
        catch
        {

        }
    }

    void Update()
    {
/*
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            //DevConsoleLogScroll.Instance.Log($"{"TestMod says"}: Yes you heard it, i am indeed a MonoBehaviour Mod!");
        }
        */
    }
}
