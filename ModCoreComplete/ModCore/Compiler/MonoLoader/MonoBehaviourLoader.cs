using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable

public class MonoBehaviourLoader : MonoBehaviour
{
    [HideInInspector] public List<GameObject> modGameObjects = new();
    private bool modsLoaded = false;
    public static MonoBehaviourLoader Instance;

    private List<string> loadErrors = new(); // Stores all load-related errors

    private bool missingDependency;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        UnloadMods();
    }

    /// <summary>
    /// Called after mods are compiled by the ModCompiler.
    /// This method is used to load or reload mods from a given compiled folder.
    /// ⚠️ Do NOT call this from Start or Awake — it is automatically invoked from the compiler after mod compilation.
    /// If you absolutely need to reload your mods in-game, please use ModCompiler.Instance.CompileMods() instead.
    /// </summary>
    public void LoadMods_PostCompile(string compiledFolder, bool calledFromCompiler = false)
    {
        if (!calledFromCompiler)
        {
            Debug.LogWarning("LoadMods should only be called from the compiler. Use calledFromCompiler = true if you're sure.");
            return;
        }

        UnloadMods();

        if (!Directory.Exists(compiledFolder))
        {
            LogError("Folder doesn't exist: " + compiledFolder);
            return;
        }

        LoadModsFromPath(compiledFolder);
        modsLoaded = true;
    }

    private void LoadModsFromPath(string folderPath)
    {
        modGameObjects.Clear();
        loadErrors.Clear(); // Clear old errors before starting a new load.

        string[] dllFiles = Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories);
        var modIdToDllPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // First pass: gather ModIDs
        foreach (string dllPath in dllFiles)
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(dllPath));
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(MonoBehaviour)) && !type.IsAbstract)
                    {
                        var modInfo = type.GetCustomAttribute<ModInfoAttribute>();
                        if (modInfo != null && !string.IsNullOrEmpty(modInfo.ModID))
                        {
                            if (!modIdToDllPath.ContainsKey(modInfo.ModID))
                                modIdToDllPath[modInfo.ModID] = dllPath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed to scan mod for ModID from {dllPath}: {ex}");
            }
        }

        // Second pass: load and validate mods
        foreach (string dllPath in dllFiles)
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(dllPath));
                foreach (Type type in assembly.GetTypes())
                {
                    if (!type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsAbstract)
                        continue;

                    var modInfo = type.GetCustomAttribute<ModInfoAttribute>();
                    if (modInfo == null)
                    {
                        SafeLogScroll($"Mod '{type.FullName}' missing [ModInfo] attribute.", Color.yellow);
                        continue;
                    }

                    var dependencies = type.GetCustomAttributes<ModDependencyAttribute>()
                                       .Select(dep => dep.ModID).ToList();

                    missingDependency = false;

                    foreach (var depModID in dependencies)
                    {
                        if (!modIdToDllPath.ContainsKey(depModID))
                        {
                            Debug.LogError($"Cannot load '{modInfo.ModID}' because dependency '{depModID}' is missing!");
                            LogError($"Cannot load '{modInfo.ModID}' because dependency '{depModID}' is missing!");
                            missingDependency = true;
                            break;
                        }
                    }

                    if (missingDependency)
                        continue;

                    string orangeHex = ColorUtility.ToHtmlStringRGB(new Color32(255, 165, 0, 255));

                    if (DevConsoleLogScroll.Instance != null)
                    {
                        DevConsoleLogScroll.Instance.Log($"<color=#{orangeHex}>Loading MonoBehaviour mod:</color> {type.FullName} (ID: {modInfo.ModID})");
                    }

                    GameObject modGO = new($"MonoMod_{type.Name}");
                    DontDestroyOnLoad(modGO);
                    modGO.AddComponent(type);
                    modGameObjects.Add(modGO);

                    SafeLogScroll($"Loaded '{modInfo.ModID}' by {modInfo.Author} (v{modInfo.Version})", Color.green);
                }
            }
            catch (Exception ex)
            {
                LogError($"Failed loading mod from {dllPath}: {ex}");
            }
        }

        if (modGameObjects.Count == 0)
        {
            SafeLogScroll("MonoBehaviour mods loading finished. Mods available to load: <color=white>None</color>", Color.green);
        }
        else
        {
            SafeLogScroll($"MonoBehaviour mods loading finished. <color=white>{modGameObjects.Count}</color> mod(s) loaded.", Color.green);
        }
    }

    public void UnloadMods()
    {
        foreach (var go in modGameObjects)
        {
            if (go != null)
                Destroy(go);
        }

        modGameObjects.Clear();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Debug.Log("Unloaded all MonoBehaviour mods.");
    }

    // --- Logging ---
    //This is made to scroll right after adding a new log.
    private void SafeLogScroll(string message, Color? color = null)
    {
        if (DevConsoleLogScroll.Instance != null)
            DevConsoleLogScroll.Instance.Log(message, color);
        else
            Debug.Log(message);
    }

    //Use this if you wanna log something without auto-scrolling.
    private void SafeLogStatic(string message, Color? color = null)
    {
        if (DevConsoleLogStatic.Instance != null)
            DevConsoleLogStatic.Instance.Log(message, color);
        else
            Debug.Log(message);
    }

    /// <summary>
    /// Logs an error both to the scroll console and to the internal error list.
    /// </summary>
    private void LogError(string message)
    {
        loadErrors.Add(message);
        SafeLogScroll(message, Color.red);
    }

    /// <summary>
    /// Gets the list of all mod loading errors encountered during the last load.
    /// </summary>
    public List<string> GetLoadErrors()
    {
        return new List<string>(loadErrors); // Return a copy to avoid external modifications
    }

}
