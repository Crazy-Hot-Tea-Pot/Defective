using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ensures that the attached GameObject persists across scenes.
/// </summary>
public class BackgroundSoundGlobal : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private List<Levels> playInScenes = new List<Levels>();
    [SerializeField]
    private List<Levels> pauseInScenes = new List<Levels>();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();
        
    }

    void Start()
    {
        // Subscribe to scene loaded event
        GameManager.Instance.OnSceneChange += SceneChange;
    }
    public void SetSceneBehavior(List<Levels> playScenes, List<Levels> pauseScenes)
    {
        playInScenes = playScenes;
        pauseInScenes = pauseScenes;
    }

    private void SceneChange(Levels newLevel)
    {
        if (pauseInScenes.Contains(newLevel))
            PauseSound();
        else
            ResumeSound();
    }
    private void PauseSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
            Debug.Log("Persistent background sound paused.");
        }
    }

    private void ResumeSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
            Debug.Log("Persistent background sound resumed.");
        }
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSceneChange -= SceneChange;
        }
    }
}
