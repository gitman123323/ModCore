using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneCommand : ConsoleCommand
{
    public override string Name => "/ChangeScene";
    public override string Description => "Changes the current scene to another one! Usage: /ChangeScene SceneName";

    public override void Execute(string[] args)
    {
        if (args.Length == 0)
        {
            DevConsoleLogScroll.Instance.Log("Usage: /ChangeScene SceneName", Color.yellow);
            return;
        }

        string sceneName = args[0];

        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            DevConsoleLogScroll.Instance.Log($"Changing scene to: {sceneName}", Color.cyan);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            DevConsoleLogScroll.Instance.Log($"Scene '{sceneName}' not found or not added to Build Settings.", Color.red);
        }
    }
}
