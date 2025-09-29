using System.Collections.Generic;
using UnityEngine;

public class NoObjectDestroyOnLoad : MonoBehaviour
{
    [Header("Assign GameObjects here that should persist across scenes")]
    public List<GameObject> objectsToPersist = new();

    void Awake()
    {
        foreach (GameObject obj in objectsToPersist)
        {
            if (obj == null)
            {
                Debug.LogWarning("One of the objects in 'objectsToPersist' is null.");
                continue;
            }

            // Check if a duplicate already exists in the scene (by name)
            GameObject existing = GameObject.Find(obj.name);

            if (existing != null && existing != obj)
            {
                Debug.Log($"Duplicate detected: Destroying extra instance of {obj.name}");
                Destroy(obj); // Destroy the duplicate
            }
            else
            {
                DontDestroyOnLoad(obj);
            }
        }
    }
}
