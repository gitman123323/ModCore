using System.IO;
using UnityEngine;

public static class ModConfig
{
    /// <summary>
    /// Returns the base path to the UserMods folder.
    /// - In Editor: "Assets/UserMods"
    /// - In Build: "[exe_folder]/UserMods"
    /// </summary>
    public static string GetModBasePath()
    {
#if UNITY_EDITOR
        // Running in the Unity Editor: use Assets/UserMods
        return Path.Combine(Application.dataPath);
#else
        // Running as a build: use the folder next to the EXE
        return Path.Combine(Path.GetDirectoryName(Application.dataPath));
#endif
    }
}
