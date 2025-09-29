using System.Collections;
using Humanizer;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleUIHook : MonoBehaviour
{
    public static ConsoleUIHook Instance;

    [Header("Console Key Settings")]
    public bool useComboKeys = false;
    public bool isDisabled;
    public KeyCode singleKey = KeyCode.F12; // Default: `
    public KeyCode comboKey1 = KeyCode.LeftControl;
    public KeyCode comboKey2 = KeyCode.RightControl;

    public GameObject consolePanel; // UI panel to show/hide
    public InputField inputField;

    private bool toggle = false;
    private bool comboPressed = false;

    // Cursor hiding logic
    public float cursorHideDelay = 3f; // seconds to wait before hiding cursor
    private Vector3 lastMousePosition;
    private float mouseStillTimer = 0f;
    private bool cursorHidden = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        /*
        try
        {
            if (DevConsoleLogScroll.Instance.gameObject != null)
            {
                Destroy(DevConsoleLogScroll.Instance.gameObject);
            }

            if (DevConsoleLogStatic.Instance.gameObject != null)
            {
                Destroy(DevConsoleLogStatic.Instance.gameObject);
            }
        }
        catch
        {
            //There is also nothing here...
        }
        */
    }
    
    void Start()
    {
        if (isDisabled)
        {
            consolePanel.SetActive(false);
        }

        if (consolePanel != null)
        {
            toggle = consolePanel.activeSelf;

            if (toggle && inputField != null)
            {
                inputField.ActivateInputField();
            }
            else if (inputField != null)
            {
                inputField.DeactivateInputField();
            }
        }

        lastMousePosition = Input.mousePosition;
        UpdateCursorVisibility();
    }

    void Update()
    {
        // Toggle console visibility input
        if (useComboKeys && !isDisabled)
        {
            if ((Input.GetKeyDown(comboKey1) && Input.GetKey(comboKey2)) ||
                (Input.GetKeyDown(comboKey2) && Input.GetKey(comboKey1)))
            {
                if (!comboPressed)
                {
                    toggle = !toggle;
                    SetConsoleVisible(toggle);
                    comboPressed = true;
                    UpdateCursorVisibility(true);
                }
            }

            if (!Input.GetKey(comboKey1) && !Input.GetKey(comboKey2))
            {
                comboPressed = false;
            }
        }
        else
        {
            if (Input.GetKeyDown(singleKey) && !isDisabled)
            {
                toggle = !toggle;
                SetConsoleVisible(toggle);
                UpdateCursorVisibility(true);
            }
        }

        if (toggle && !inputField.isFocused)
        {
            inputField.ActivateInputField();
            StartCoroutine(DeselectTextNextFrame(inputField));
        }
        else if (!toggle && inputField.isFocused)
        {
            inputField.DeactivateInputField();
        }

        // Cursor hide/show management only if console is closed
        if (!toggle)
        {
            Vector3 currentMousePos = Input.mousePosition;
            if (currentMousePos != lastMousePosition)
            {
                // Mouse moved — show cursor and reset timer
                mouseStillTimer = 0f;
                if (cursorHidden)
                {
                    Cursor.visible = true;
                    cursorHidden = false;
                }
            }
            else
            {
                // Mouse still — increment timer
                mouseStillTimer += Time.deltaTime;
                if (mouseStillTimer >= cursorHideDelay && !cursorHidden)
                {
                    Cursor.visible = false;
                    cursorHidden = true;
                }
            }

            lastMousePosition = currentMousePos;
        }
        else
        {
            // If console open, always show cursor
            if (cursorHidden)
            {
                Cursor.visible = true;
                cursorHidden = false;
            }
            mouseStillTimer = 0f;
            lastMousePosition = Input.mousePosition;
        }
    }

    private IEnumerator DeselectTextNextFrame(InputField inputField)
    {
        yield return null; // wait a frame so Unity doesn't override it

        // Move caret to the end without selecting
        inputField.MoveTextEnd(false);
    }


    public void OnSubmit()
    {
        string input = inputField.text;
        if (CommandRegistry.Instance.TryExecuteCommand(input))
        {
            Debug.Log($"Command Executed Successfully!");
        }

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void SetConsoleVisible(bool visible)
    {
        if (consolePanel != null)
            consolePanel.SetActive(visible);
    }

    private void UpdateCursorVisibility(bool immediateShow = false)
    {
        if (toggle || immediateShow)
        {
            Cursor.visible = true;
            cursorHidden = false;
            mouseStillTimer = 0f;
        }
        else
        {
            Cursor.visible = false;
            cursorHidden = true;
        }
    }
}
