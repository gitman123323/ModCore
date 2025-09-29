using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class SceneChangeListener : MonoBehaviour
{
    public static SceneChangeListener Instance { get; private set; }

    private Scene currentScene;
    private bool initialized = false; // Track if first load has passed

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[Scene Loaded] Name: {scene.name} | Mode: {mode}");

        // Update the current scene but ignore triggering logic
        currentScene = scene;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        if (!initialized)
        {
            // Skip the first automatic call
            initialized = true;
            currentScene = newScene;
            Debug.Log("[SceneChangeListener] Initial scene detected. Ignoring first call.");
            return;
        }

        Debug.Log($"[Scene Changed] From: {oldScene.name} → To: {newScene.name}");

        currentScene = newScene;
        TriggerOnceOnSceneChange();
    }

    private void TriggerOnceOnSceneChange()
    {
        DevConsoleLogScroll.Instance.Log($"Scene changed to '{currentScene.name}' sucessfully!", Color.green);
    }
}
