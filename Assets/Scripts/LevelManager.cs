using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Level Configuration")]
    public List<GameObject> levelPrefabs = new List<GameObject>();
    //public Transform levelParent;

    [Header("Current State")]
    public int currentLevelIndex = 0;
    private GameObject currentLevelObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (levelPrefabs.Count > 0)
        {
            LoadLevel(currentLevelIndex);
        }
    }

    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelPrefabs.Count)
        {
            Debug.LogError("Level index out of range!");
            return;
        }

        // Cleanup existing level
        if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
        }

        // Wait a frame if needed, or instantiate immediately. 
        // Immediate instantiation is usually fine if the old one is destroyed.
        // However, since PuzzleManager is a singleton, we need to ensure the old one's OnDestroy runs first.
        // Unity destroys objects at the end of the frame usually, but we forced Instance = null in OnDestroy.
        // Let's rely on Unity's lifecycle.

        StartCoroutine(LoadLevelRoutine(index));
    }

    private IEnumerator LoadLevelRoutine(int index)
    {
        // If we just destroyed the object, wait for end of frame to ensure cleanup
        if (currentLevelObject != null)
        {
            // The Destroy call above marks it. Wait for it to actually be gone?
            // Actually, Destroy() happens at end of frame.
            yield return new WaitForEndOfFrame();
        }

        currentLevelIndex = index;
        GameObject prefab = levelPrefabs[currentLevelIndex];

        currentLevelObject = Instantiate(prefab);//levelParent

        // Reset or init UI if needed
        Debug.Log($"Loaded Level {index + 1}");
    }

    [ContextMenu("NextLevel")]
    public void NextLevel()
    {
        int nextIndex = currentLevelIndex + 1;
        if (nextIndex < levelPrefabs.Count)
        {
            LoadLevel(nextIndex);
        }
        else
        {
            Debug.Log("No more levels!");
            // Optional: return to menu or loop, or just stay
        }
    }

    public void RestartLevel()
    {
        LoadLevel(currentLevelIndex);
    }
}
