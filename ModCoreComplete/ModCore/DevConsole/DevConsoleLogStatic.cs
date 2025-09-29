using UnityEngine;
using UnityEngine.UI;

public class DevConsoleLogStatic : MonoBehaviour
{
    public static DevConsoleLogStatic Instance;
    private static readonly NullConsoleFallbackStatic _nullInstance;
    public Text consoleText;
    public RectTransform contentRect;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        try
        {
            Instance = _nullInstance; // 👈 Replace with dummy, not null
        }
        catch
        {
            //There is nothing here...
        }
    }

    void Update()
    {

    }

    /// <summary>
    /// Logs a message to the console with optional color (excludes >> prefix).
    /// </summary>
    public void Log(string message, Color? color = null)
    {
        string coloredMessage = message;

        if (color.HasValue)
        {
            string hexColor = ColorUtility.ToHtmlStringRGBA(color.Value);
            coloredMessage = $"<color=#{hexColor}>{message}</color>";
        }

        consoleText.text += "\n >> " + coloredMessage;
    }
}
