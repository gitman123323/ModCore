=== ConsoleUIHook ===

Summary:
ConsoleUIHook manages the visibility and behavior of the in-game console UI. It listens for toggle input via a single key or key combination and provides cursor visibility management for user experience polish.

Features:

Singleton pattern (ConsoleUIHook.Instance) for global access.

Toggle console with:

A single key (e.g. F12), or

A combination (e.g. LeftCtrl + RightCtrl).

Manages input focus with InputField.

Automatically hides the mouse cursor when inactive.

Fully customizable keybindings.

Automatically reactivates input field if console is toggled on.

Notifies CommandRegistry when the user submits input.

Usage Notes:

Attach this script to a persistent UI GameObject in the scene.

Set up references to the consolePanel and inputField in the Inspector.

Use OnSubmit to send user-entered text to the command system.

Cursor automatically hides after inactivity when the console is not visible.

Combo key logic prevents repeated toggles during a single press.

Key Variables:

consolePanel: The panel that contains your input and log UI.

inputField: Unity UI InputField for user commands.

cursorHideDelay: Seconds before the mouse cursor is hidden when idle.

Example:

void Update()
{
    if (Input.GetKeyDown(KeyCode.F12))
    {
        toggle = !toggle;
        SetConsoleVisible(toggle);
    }
}

public void OnSubmit()
{
    string input = inputField.text;
    CommandRegistry.Instance.TryExecuteCommand(input);
    inputField.text = "";
    inputField.ActivateInputField();
}

Advanced Tip:
Use the optional Humanizer package (imported in the script) for pretty-printing or string formatting in logs and future extensions.