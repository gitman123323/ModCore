=== CoroutineRunner ===

Summary:
CoroutineRunner is an internal singleton MonoBehaviour that enables coroutine execution
outside of standard MonoBehaviour classes. It is responsible for actually starting and
stopping coroutines behind the scenes.

Behavior:
- If no instance exists, a new GameObject named "CoroutineRunner" is created at runtime.
- The GameObject is marked as DontDestroyOnLoad so it persists between scenes.

Methods:
- static Coroutine Start(IEnumerator coroutine)
  Starts a coroutine through the singleton instance.

- static void Stop(Coroutine coroutine)
  Stops a coroutine that was previously started.

Important Notes:
- This class is meant to be used **internally** by the Coroutines utility.
- You should **avoid calling CoroutineRunner directly** in your own code.
  Doing so adds unnecessary verbosity and reduces readability.
- For cleaner syntax, always use the Coroutines utility instead.

---

=== Coroutines ===

Summary:
Coroutines is a static utility class that allows you to start and stop coroutines from anywhere,
even outside of MonoBehaviour classes. It provides a clean and readable way to work with coroutines
using Unity-style syntax.

Usage:
At the top of your script, add:
using static Coroutines;

You can then start or stop coroutines exactly like you would inside a MonoBehaviour:

Example:
StartCoroutine(MyCoroutine());
StopCoroutine(myCoroutineHandle);

Methods:
- static Coroutine StartCoroutine(IEnumerator coroutine)
  Starts the given coroutine using CoroutineRunner internally.

- static void StopCoroutine(Coroutine coroutine)
  Stops the given coroutine if it’s still running.

Best Practices:
- Always prefer using `using static Coroutines;` for clean and familiar syntax.
- Avoid referencing CoroutineRunner directly unless you have a very specific need.
- This approach keeps your code concise and decoupled from GameObject management.
