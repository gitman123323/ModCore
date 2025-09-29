=== NoObjectDestroyOnLoad ===

Purpose

The NoObjectDestroyOnLoad script ensures that specific GameObjects persist across scene loads in Unity, while avoiding duplicate instances. This is helpful for objects like managers, UI components, or audio sources that need to exist throughout the game lifecycle.

Usage

Attach this script to a GameObject in your initial scene.

In the Inspector, populate the objectsToPersist list with the GameObjects you want to persist across scenes.

Key Features

Prevents duplicates: If an object with the same name already exists, the new one will be destroyed to prevent duplication.

Persists objects: Valid objects are marked with DontDestroyOnLoad, keeping them alive between scene transitions.

Null safety: Logs warnings for any unassigned (null) entries in the list.

Code Breakdown

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

Notes

Unity's DontDestroyOnLoad only works on root GameObjects. Ensure the GameObjects in the list are not children unless intentionally designed.

Name-based duplicate detection may not be ideal if multiple objects share names. Consider extending the check with custom identifiers if needed.

This script helps maintain clean state management across scenes without manually handling singleton lifecycles.