using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ensures that the attached GameObject persists across scenes.
/// </summary>
public class BackgroundSoundGlobal : MonoBehaviour
{
    public static BackgroundSoundGlobal Instance
    {
        get;
        private set;
    }
    private AudioSource audioSource;
    private AudioClip previousClip;
    //private bool wasPlayingBeforeCombat = false;
    [SerializeField]
    private List<Levels> playInScenes = new List<Levels>();
    [SerializeField]
    private List<Levels> pauseInScenes = new List<Levels>();

    void Awake()
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
        }
    }

    private void ResumeSound()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();            
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
