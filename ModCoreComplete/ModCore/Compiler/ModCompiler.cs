using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public class ModCompiler : MonoBehaviour
{
    public static ModCompiler Instance;
    [HideInInspector] public SpriteRenderer player;

    private List<MetadataReference> references;
    private Dictionary<string, DateTime> lastCompileTimes = new();
    private SemaphoreSlim concurrencySemaphore;

    // ------------------------ Unity Lifecycle ------------------------

    private void Awake()
    {
        references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>()
            .ToList();

        concurrencySemaphore = new SemaphoreSlim(Environment.ProcessorCount);

        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        bool scrollMissing = DevConsoleLogScroll.Instance == null;
        bool staticMissing = DevConsoleLogStatic.Instance == null;

        if (scrollMissing && staticMissing)
        {
            Debug.LogWarning("⚠️ Missing console components: 'DevConsoleLogScroll' and 'DevConsoleLogStatic'. Falling back to direct mod compilation.");

            if (ConsoleUIHook.Instance != null)
                Destroy(ConsoleUIHook.Instance.gameObject);

            CompileMods();
        }
        else
        {
            StartCoroutine(LoadConsole());
        }
    }

    private IEnumerator LoadConsole()
    {
        string[] lines = DevConsoleLogStatic.Instance.consoleText.text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(">> Initializing Console..."))
            {
                string orangeHex = ColorUtility.ToHtmlStringRGB(new Color32(255, 165, 0, 255));
                string greenHex = ColorUtility.ToHtmlStringRGB(Color.green);

                lines[i] = $" >> <color=#{orangeHex}>Initializing Console...</color>";
                DevConsoleLogStatic.Instance.consoleText.text = string.Join("\n", lines);

                yield return new WaitForSeconds(0.7f);

                lines[i] = $" >> <color=#{greenHex}>Console Initialized...</color>";
                DevConsoleLogStatic.Instance.consoleText.text = string.Join("\n", lines);

                StartCoroutine(Load());
                break;
            }
        }
    }

    private IEnumerator Load()
    {
        yield return new WaitForSeconds(0.1f);
        DevConsoleLogStatic.Instance.Log("Compiling Mods...", Color.cyan);
        yield return new WaitForSeconds(0.2f);
        CompileMods();
    }

    // ------------------------ Compilation Logic ------------------------

    //[ContextMenu("Compile All Mods")]
    public async void CompileMods(string customSuccessMessage = null)
    {
        string[] modTypes = { "IMods", "MonoBehaviourMods" };
        List<Task> compileTasks = new();
        int compiledModCount = 0;

        foreach (var modType in modTypes)
        {
            string modsRoot = Path.Combine(ModConfig.GetModBasePath(), "UserMods", modType);
            string compiledFolder = Path.Combine(modsRoot, "Compiled");
            string disabledModsFolder = Path.Combine(modsRoot, "DisabledMods");
            Directory.CreateDirectory(compiledFolder);
            Directory.CreateDirectory(disabledModsFolder);

            foreach (var dir in Directory.GetDirectories(modsRoot))
            {
                string modName = Path.GetFileName(dir);
                if (modName == "Compiled") continue;

                if (modName == "DisabledMods")
                {
                    bool hasContent = Directory.EnumerateFileSystemEntries(disabledModsFolder).Any();
                    Debug.Log(hasContent
                        ? $"Skipping '{modType}/DisabledMods' folder — Mods inside are disabled."
                        : $"'{modType}/DisabledMods' folder is empty — nothing to skip.");
                    continue;
                }

                /*
                if (!HasModChanged(dir))
                {
                    Debug.Log($"Skipping foldered mod '{modName}' in '{modType}' — no changes detected.");
                    continue;
                }
                */

                compiledModCount++;
                compileTasks.Add(CompileModFolderAsync(dir, compiledFolder, modName));
            }

            foreach (var file in Directory.GetFiles(modsRoot, "*.cs", SearchOption.TopDirectoryOnly))
            {
                if (!HasModChanged(file))
                {
                    Debug.Log($"Skipping loose file '{Path.GetFileNameWithoutExtension(file)}' in '{modType}' — no changes detected.");
                    continue;
                }

                compiledModCount++;
                compileTasks.Add(CompileLooseFileAsync(file, compiledFolder));
            }
        }

        await Task.WhenAll(compileTasks);
        Debug.Log("All mod compilation tasks completed.");

        if (compiledModCount > 0)
        {
            string successMsg = customSuccessMessage ?? "All mods were successfully compiled!";
            SafeLogStatic(successMsg, Color.cyan);
        }
        else
        {
            SafeLogStatic("No mods were compiled. Please check the DisabledMods folder in both mod types", Color.yellow);
        }

        string imodsCompiled = Path.Combine(ModConfig.GetModBasePath(), "UserMods", "IMods", "Compiled");
        string monoModsCompiled = Path.Combine(ModConfig.GetModBasePath(), "UserMods", "MonoBehaviourMods", "Compiled");

        if (ModLoader.Instance != null)
        {
            ModLoader.Instance.LoadMods_PostCompile(imodsCompiled, true);
        }

        if (MonoBehaviourLoader.Instance != null)
        {
            MonoBehaviourLoader.Instance.LoadMods_PostCompile(monoModsCompiled, true);
        }
    }


    private async Task CompileModFolderAsync(string dir, string compiledFolder, string modName)
    {
        await concurrencySemaphore.WaitAsync();
        try
        {
            string[] csFiles = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);
            if (csFiles.Length == 0) return;

            // Read all files in parallel
            var readTasks = csFiles.Select(f => File.ReadAllTextAsync(f)).ToArray();
            var sources = await Task.WhenAll(readTasks);

            var syntaxTrees = sources.Select(source => CSharpSyntaxTree.ParseText(source)).ToList();

            string modOutDir = Path.Combine(compiledFolder, modName);
            Directory.CreateDirectory(modOutDir);
            string outputPath = Path.Combine(modOutDir, $"{modName}.dll");

            var compilation = CSharpCompilation.Create(modName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithConcurrentBuild(true))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTrees);

            using var fs = File.Create(outputPath);
            var result = compilation.Emit(fs);

            if (result.Success)
            {
                Debug.Log($"✅Compiled foldered mod '{modName}'");
                SafeLogScroll($"✅Compiled foldered mod '{modName}'", Color.green);
            }
            else
            {
                Debug.LogError($"❌Failed to compile mod folder '{modName}':");
                SafeLogScroll($"❌Failed to compile mod folder '{modName}':", Color.red);
                foreach (var diag in result.Diagnostics)
                {
                    Debug.LogError(diag.ToString());
                    SafeLogScroll(diag.ToString(), Color.red);
                }
            }
        }
        finally
        {
            concurrencySemaphore.Release();
        }
    }

    private async Task CompileLooseFileAsync(string file, string compiledFolder)
    {
        await concurrencySemaphore.WaitAsync();
        try
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string source = await File.ReadAllTextAsync(file);
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            string outputPath = Path.Combine(compiledFolder, $"{fileName}.dll");

            var compilation = CSharpCompilation.Create(fileName)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithConcurrentBuild(true))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);

            using var fs = File.Create(outputPath);
            var result = compilation.Emit(fs);

            if (result.Success)
            {
                Debug.Log($"✅Compiled loose file '{fileName}'");
                SafeLogScroll($"✅Compiled loose file '{fileName}'", Color.green);
            }
            else
            {
                Debug.LogError($"❌Failed to compile loose file '{fileName}':");
                SafeLogScroll($"❌Failed to compile loose file '{fileName}':", Color.red);
                foreach (var diag in result.Diagnostics)
                {
                    Debug.LogError(diag.ToString());
                    SafeLogScroll(diag.ToString(), Color.red);
                }
            }
        }
        finally
        {
            concurrencySemaphore.Release();
        }
    }

    private bool HasModChanged(string path)
    {
        DateTime lastWrite;

        if (Directory.Exists(path))
        {
            var files = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
            if (files.Length == 0) return false;
            lastWrite = files.Max(f => File.GetLastWriteTimeUtc(f));
        }
        else if (File.Exists(path))
        {
            lastWrite = File.GetLastWriteTimeUtc(path);
        }
        else
        {
            return false;
        }

        if (lastCompileTimes.TryGetValue(path, out DateTime lastCompile))
        {
            if (lastWrite <= lastCompile)
                return false;
        }

        lastCompileTimes[path] = DateTime.UtcNow;
        return true;
    }

    // ------------------------ Logging Helpers ------------------------

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
