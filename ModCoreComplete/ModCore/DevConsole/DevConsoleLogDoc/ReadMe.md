=== DevConsoleLogScroll ===

Summary

DevConsoleLogScroll is a dynamic console logging component designed to manage scrolling text logs in a Unity UI.
It handles user input scrolling, smooth auto-scroll, and log clearing. It provides visual feedback to developers or users during runtime debugging.

Features

Singleton access via Instance.

Auto-scrolls when new messages are logged.

Supports colored messages using Unity rich text tags.

Detects manual scroll input and disables auto-scroll temporarily.

Smooth scrolling via Mathf.SmoothDamp.

Resets console and scroll position when cleared.

Logs messages with or without color.

Fields

consoleText: Text UI component displaying the log.

content: RectTransform of scrollable content.

scrollY: Current target scroll position.

scrollSpeed: Speed of user scroll with mouse wheel.

maxAutoScrollDistance: Distance added to auto-scroll to keep view pinned.

viewportRect: Cached reference to the viewable area.

forceScrollToBottom: Ensures scroll remains at the bottom when not user-scrolled.

Example Usage

DevConsoleLogScroll.Instance.Log("Game started.", Color.green);

To clear:

DevConsoleLogScroll.Instance.ResetAndClearConsole();

To display error:

DevConsoleLogScroll.Instance.Log("Failed to connect", Color.red);

DevConsoleLogStatic

Summary

DevConsoleLogStatic is a minimal logging utility similar to DevConsoleLogScroll, but without scrolling behavior. It is used for static, immediate logs to a UI text field.

Features

Singleton instance for global access.

Logs messages to UI Text field.

Supports color-coded messages.

Fallback instance prevents errors when destroyed.

Fields

consoleText: The Text UI object where logs are shown.

contentRect: RectTransform for UI layout adjustment (not currently used actively).

Example Usage

DevConsoleLogStatic.Instance.Log("Initialized", Color.cyan);

Notes

Both classes support optional fallback mechanisms (NullConsoleFallback) to safely handle destruction without null exceptions.

Use DevConsoleLogStatic for simpler logs and DevConsoleLogScroll when scrollable logs are needed.