using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DevConsoleLogScroll : MonoBehaviour
{
    public static DevConsoleLogScroll Instance;
    private static readonly NullConsoleFallback _nullInstance;
    public Text consoleText;
    public float maxAutoScrollDistance = 5f;
    private RectTransform viewportRect;
    public RectTransform content;

    private float scrollY; // Target scroll position (autoscroll)
    private float currentVelocity = 0f;
    private float smoothTime = 0.15f; // Smoothness factor (adjust for more or less smoothing)
    public float scrollSpeed = 50f;  // Speed for mouse wheel

    private bool userScrolled = false;
    private bool forceScrollToBottom = false;

    private Vector2 originalOffsetMin;
    private Vector2 originalOffsetMax;

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

    void Start()
    {
        originalOffsetMin = content.offsetMin;
        originalOffsetMax = content.offsetMax;

        content.offsetMin = originalOffsetMin;
        content.offsetMax = originalOffsetMax;
        viewportRect = content;
    }

    public void Log(string message)
    {
        Log(message, null);
    }

    public void Log(string message, Color? color)
    {
        string formattedMessage = "\n >> ";

        if (color.HasValue)
        {
            Color col = color.Value;
            string hexColor = ColorUtility.ToHtmlStringRGB(col);
            formattedMessage += $"<color=#{hexColor}>{message}</color>";
        }
        else
        {
            formattedMessage += message;
        }

        consoleText.text += formattedMessage;
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        StartCoroutine(AdjustScroll());
    }

    private IEnumerator AdjustScroll()
    {
        yield return null;

        float textHeight = consoleText.preferredHeight;
        float viewportHeight = viewportRect.rect.height;

        // Add 1 pixel padding to ensure no partial line shows
        scrollY = Mathf.Max(0, textHeight - viewportHeight + maxAutoScrollDistance);

        float currentY = content.anchoredPosition.y;
        float nearBottomThreshold = 10f;

        if (currentY < scrollY - nearBottomThreshold || !userScrolled)
        {
            forceScrollToBottom = true;
        }
    }

    void Update()
    {
        // Manual scroll via mouse
        if (Input.mouseScrollDelta.y != 0)
        {
            userScrolled = true;
            float delta = Input.mouseScrollDelta.y * scrollSpeed;
            scrollY = Mathf.Clamp(content.anchoredPosition.y - delta, 0, GetMaxScroll() + maxAutoScrollDistance);
            forceScrollToBottom = false;
        }

        // If forced scroll to bottom
        if (forceScrollToBottom)
        {
            scrollY = GetMaxScroll() + maxAutoScrollDistance; // Always scroll 1px lower to hide top partial
        }

        // Smooth scroll movement
        float newY = Mathf.SmoothDamp(content.anchoredPosition.y, scrollY, ref currentVelocity, smoothTime);
        content.anchoredPosition = new Vector2(0, newY);

        // Reset auto-scroll lock if near bottom
        if (Mathf.Abs(content.anchoredPosition.y - GetMaxScroll()) < 10f)
        {
            userScrolled = false;
        }

        ClampContentPosition();
    }

    private float GetMaxScroll()
    {
        float textHeight = consoleText.preferredHeight;
        float viewportHeight = viewportRect.rect.height;
        return Mathf.Max(0, textHeight - viewportHeight);
    }

    private void ClampContentPosition()
    {
        Vector2 pos = content.anchoredPosition;
        float maxY = GetMaxScroll();
        pos.y = Mathf.Clamp(pos.y, 0, maxY + maxAutoScrollDistance); // Clamp with +1 padding for hiding top partial
        content.anchoredPosition = pos;
    }

    public void ResetAndClearConsole()
    {
        content.offsetMin = originalOffsetMin;
        content.offsetMax = originalOffsetMax;
        consoleText.text = "";
        scrollY = 0;
        content.anchoredPosition = Vector2.zero;
    }
}
