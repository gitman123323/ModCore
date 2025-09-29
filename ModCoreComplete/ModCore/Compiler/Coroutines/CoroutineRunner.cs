using UnityEngine;
using System.Collections;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("CoroutineRunner");
                _instance = obj.AddComponent<CoroutineRunner>();
                GameObject.DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    public static Coroutine Start(IEnumerator coroutine)
    {
        return Instance.StartCoroutine(coroutine);
    }

    public static void Stop(Coroutine coroutine)
    {
        if (_instance != null)
            _instance.StopCoroutine(coroutine);
    }
}
