using UnityEngine;
using TMPro;
using Debug = UnityEngine.Debug;

#pragma warning disable

[ModInfo("FPSCounterMod", "NullCore-Systems", "1.0.0")]
public class FPSCounterMod : IMod
{
    private static TextMeshProUGUI text;
    private float deltaTime;
    private bool wantToUnload = false;

    public void OnLoad()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 60;

        GameObject textObj = GameObject.FindGameObjectWithTag("FPSText");
        if (textObj != null)
        {
            text = textObj.GetComponent<TextMeshProUGUI>();
            text.text = "FPS: Calculating...";
        }
        else
        {
            Debug.LogWarning("FPSText object not found. FPS display will not work.");
        }
    }

    public void OnUpdate()
    {
        /*
        if (Input.GetKeyDown(KeyCode.I))
            wantToUnload = true;
            */

        if (text != null)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            text.text = $"FPS: {Mathf.Ceil(fps)}";
        }
    }

    public void OnFixedUpdate()
    {
        
    }

    public void OnUnload()
    {
        Debug.Log("MyMod2 was unloaded successfully!");
    }

    public bool ShouldUnload()
    {
        return wantToUnload;
    }
}
