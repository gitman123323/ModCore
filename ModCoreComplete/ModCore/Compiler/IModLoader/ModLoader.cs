using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

#pragma warning disable
public class ModLoader : MonoBehaviour
{
    [HideInInspector]
    public List<IMod> loadedMods = new List<IMod>();
    private bool modsLoaded = false;

    public static ModLoader Instance;

    private List<string> loadErrors = new(); // Stores all load-related errors


    private bool missingDependency;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Iterate in reverse so we can safely remove mods during the loop
        for (int i = loadedMods.Count - 1; i >= 0; i--)
        {
            var mod = loadedMods[i];
            try
            {
                mod.OnUpdate();

                if (mod.ShouldUnload())
                {
                    mod.OnUnload();
                    loadedMods.RemoveAt(i);
                    Debug.Log($"Mod {mod} unloaded itself.");
                    SafeLogScroll($"Mod {mod} unloaded itself.", Color.yellow);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Mod {mod.GetType().Name} threw an error during Update: {ex}");
                LogError($"Mod {mod.GetType().Name} threw an error during Update: {ex}");
                try
                {
                    mod.OnUnload();
                }
                catch (Exception unloadEx)
                {
                    LogError($"Error while unloading faulty mod {mod.GetType().Name}: {unloadEx}");
                    Debug.LogError($"Error while unloading faulty mod {mod.GetType().Name}: {unloadEx}");
                }
                loadedMods.RemoveAt(i);
            }
        }
    }

    void FixedUpdate()
    {
        for (int i = loadedMods.Count - 1; i >= 0; i--)
        {
            var mod = loadedMods[i];
            try
            {
                mod.OnFixedUpdate();
            }
            catch (Exception ex)
            {
                LogError($"Mod {mod.GetType().Name} threw an error during FixedUpdate: {ex}");
            }
        }
    }

    void LateUpdate()
    {
        for (int i = loadedMods.Count - 1; i >= 0; i--)
        {
            var mod = loadedMods[i];
            try
            {
                mod.OnLateUpdate();
            }
            catch (Exception ex)
            {
                LogError($"Mod {mod.GetType().Name} threw an error during LateUpdate: {ex}");
            }
        }
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
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

        foreach (var mod in loadedMods)
        {
            GUIDispatcher.Instance.Unregister(mod.OnGUI);
        }
        loadedMods.Clear();
        loadErrors.Clear();

        if (!Directory.Exists(compiledFolder))
        {
            Debug.LogError($"Folder doesn't exist: {compiledFolder}");
            LogError($"Folder doesn't exist: {compiledFolder}");
            return;
        }

        string[] dllFiles = Directory.GetFiles(compiledFolder, "*.dll", SearchOption.AllDirectories);
        var modIdToDllPath = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var modsToLoad = new List<IMod>();

        // First pass: scan and map ModIDs
        foreach (string dllPath in dllFiles)
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(dllPath));
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IMod).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        var modInfo = type.GetCustomAttribute<ModInfoAttribute>();
                        if (modInfo != null && !string.IsNullOrEmpty(modInfo.ModID))
                        {
                            if (!modIdToDllPath.ContainsKey(modInfo.ModID))
                                modIdToDllPath.Add(modInfo.ModID, dllPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to scan mod for ModID from {dllPath}: {ex}");
                LogError($"Failed to scan mod for ModID from {dllPath}: {ex}");
            }
        }

        // Second pass: load mods and validate dependencies
        foreach (string dllPath in dllFiles)
        {
            try
            {
                var assembly = Assembly.Load(File.ReadAllBytes(dllPath));
                foreach (Type type in assembly.GetTypes())
                {
                    if (!typeof(IMod).IsAssignableFrom(type) || type.IsInterface || type.IsAbstract)
                        continue;

                    var modInfo = type.GetCustomAttribute<ModInfoAttribute>();
                    if (modInfo == null)
                    {
                        Debug.LogWarning($"Mod '{type.FullName}' is missing [ModInfo] attribute.");
                        SafeLogScroll($"Mod '{type.FullName}' is missing [ModInfo] attribute.", Color.yellow);
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
                        DevConsoleLogScroll.Instance.Log($"<color=#{orangeHex}>Loading IMod mod:</color> {type.FullName} (ID: {modInfo.ModID})");
                    }

                    IMod modInstance = (IMod)Activator.CreateInstance(type);
                    modsToLoad.Add(modInstance);
                    Debug.Log($"Loaded mod '{modInfo.ModID}' by {modInfo.Author} (v{modInfo.Version})");
                    SafeLogScroll($"Loaded mod '{modInfo.ModID}' by {modInfo.Author} (v{modInfo.Version})", Color.green);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed loading mod from {dllPath}: {ex}");
                LogError($"Failed loading mod from {dllPath}: {ex}");
            }
        }

        // Final step: initialize loaded mods
        foreach (var mod in modsToLoad)
        {
            try
            {
                mod.OnLoad();
                loadedMods.Add(mod);
                GUIDispatcher.Instance.Register(mod.OnGUI);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading mod: {mod.GetType().FullName}: {ex}");
                LogError($"Error loading mod: {mod.GetType().FullName}: {ex}");
            }
        }

        if (loadedMods.Count == 0)
        {
            SafeLogScroll("IMods loading finished. Mods available to load: <color=white>None</color>", Color.green);
        }
        else
        {
            SafeLogScroll($"IMods loading finished. <color=white>{loadedMods.Count}</color> mod(s) loaded.", Color.green);
        }
    }


    public void UnloadMods()
    {
        foreach (var mod in loadedMods)
        {
            try
            {
                Debug.Log($"Unloading mod: {mod.GetType().Name}");
                //DevConsoleLogScroll.Instance.Log($"Unloading mod: {mod.GetType().Name}", Color.yellow);
                GUIDispatcher.Instance.Unregister(mod.OnGUI);
                mod.OnUnload();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error unloading mod: {ex}");
                LogError($"Error unloading mod: {ex}");
            }
        }
        loadedMods.Clear();

        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }

    // --- Helper methods for safe logging with fallback ---

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
