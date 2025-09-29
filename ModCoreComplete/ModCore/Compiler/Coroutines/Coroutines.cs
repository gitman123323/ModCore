using UnityEngine;
using System.Collections;

public static class Coroutines
{
    public static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return CoroutineRunner.Start(coroutine);
    }

    public static void StopCoroutine(Coroutine coroutine)
    {
        CoroutineRunner.Stop(coroutine);
    }
}
