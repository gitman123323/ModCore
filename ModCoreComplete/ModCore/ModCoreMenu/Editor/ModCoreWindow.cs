using UnityEngine;
using UnityEditor;
using System.IO;

public class ModCoreWindow : EditorWindow
{
    [MenuItem("Tools/Mod Core")]
    public static void ShowWindow()
    {
        GetWindow<ModCoreWindow>("ModCore");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Console Command", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Command Script"))
        {
            CommandNamePromptWindow.Open(CreateCommandScript);
        }
    }

    private void CreateCommandScript(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            Debug.LogWarning("Command name cannot be empty.");
            return;
        }

        string folderPath = "Assets/ModCore/DevConsole/CommandSystem/Commands";
        string filePath = Path.Combine(folderPath, $"{className}.cs");

        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        if (File.Exists(filePath))
        {
            Debug.LogWarning($"File already exists: {filePath}");
            return;
        }

        string scriptContent = $@"using System;
    using UnityEngine;

    public class {className} : ConsoleCommand
    {{
        public override string Name => ""/{className.ToLower().Replace("command", "")}"";
        public override string Description => ""Describe what this command does"";

        public override void Execute(string[] args)
        {{
            try
            {{
                DevConsoleLogScroll.Instance.Log(""<color=#DDA0DD>Hello, Dev!</color>"");
            }}
            catch (Exception ex)
            {{
                DevConsoleLogScroll.Instance.Log($""{{ex.Message}}"", Color.red);
            }}
        }}
    }}";
        Close();

        File.WriteAllText(filePath, scriptContent);
        AssetDatabase.Refresh();

        Debug.Log($"Created command script: {filePath}");
    }
}

public class CommandNamePromptWindow : EditorWindow
{
    private static System.Action<string> onSubmit;
    private string inputName = "";

    public static void Open(System.Action<string> submitCallback)
    {
        var window = CreateInstance<CommandNamePromptWindow>();
        window.titleContent = new GUIContent("Enter Command Name");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 80);
        window.minSize = new Vector2(300, 80);
        window.maxSize = new Vector2(300, 80);
        onSubmit = submitCallback;
        window.ShowUtility();
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter new command class name:", EditorStyles.wordWrappedLabel);
        inputName = EditorGUILayout.TextField(inputName);

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create"))
        {
            if (!string.IsNullOrEmpty(inputName))
            {
                onSubmit?.Invoke(inputName.Trim());
                Close();
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid Name", "Please enter a valid command name.", "OK");
            }
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }

        GUILayout.EndHorizontal();
    }
}
