using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class CreditsController : MonoBehaviour
{
    public TMP_Text m_TextComponent;

    [Range(0.000f,0.100f)]
    public float revealSpeed = 0.05f; 
    public int wordsPerCycle = 1;
    public bool Wordmode;
    public PlayerInputActions playerInputActions;
    public VideoPlayer VideoPlayer;

    private bool hasTextChanged;
    private InputAction backToTitleAction;
    private List<string> recordedVideos;
    private int currentVideoIndex = 0;

    void OnEnable()
    {
        backToTitleAction = playerInputActions.Player.Escape;

        backToTitleAction.Enable();

        backToTitleAction.performed += BackToTitle;
    }

    void Awake()
    {
        // assign Player Input class
        playerInputActions = new PlayerInputActions();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(Wordmode)
            StartCoroutine(RevealWords(m_TextComponent));
        else
            StartCoroutine(RevealCharacters(m_TextComponent));

        LoadRecordedVideos();
    }
    private void LoadRecordedVideos()
    {
        recordedVideos = DataManager.Instance.CurrentGameData.SecurityCamRecordings;
        if (recordedVideos.Count > 0)
        {
            PlayNextVideo();
        }
    }
    private IEnumerator PlayVideo(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        List<Texture2D> frames = DecodeVidFile(fileBytes);

        foreach (Texture2D frame in frames)
        {
            VideoPlayer.targetTexture = new RenderTexture(frame.width, frame.height, 0);
            Graphics.Blit(frame, VideoPlayer.targetTexture);
            yield return new WaitForSeconds(1f / 30f); // Assuming 30 FPS playback
        }

        NextVideo();
    }

    private void NextVideo()
    {
        currentVideoIndex = (currentVideoIndex + 1) % recordedVideos.Count;
        PlayNextVideo();
    }

    private List<Texture2D> DecodeVidFile(byte[] fileBytes)
    {
        List<Texture2D> frames = new List<Texture2D>();
        using (MemoryStream stream = new MemoryStream(fileBytes))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            string identifier = new string(reader.ReadChars(4));
            if (identifier != "VIDF") return frames;

            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            int frameCount = reader.ReadInt32();

            for (int i = 0; i < frameCount; i++)
            {
                int frameSize = reader.ReadInt32();
                byte[] frameData = reader.ReadBytes(frameSize);
                Texture2D frameTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
                frameTexture.LoadImage(frameData);
                frames.Add(frameTexture);
            }
        }
        return frames;
    }

    private void PlayNextVideo()
    {
        if (recordedVideos.Count == 0) return;

        string videoPath = recordedVideos[currentVideoIndex];
        if (File.Exists(videoPath))
        {
            StartCoroutine(PlayVideo(videoPath));
        }
        else
        {
            Debug.LogWarning($"Video file not found: {videoPath}");
            NextVideo();
        }
    }
    private void BackToTitle(InputAction.CallbackContext context)
    {
        GameManager.Instance.RequestScene(Levels.Title);
    }
    IEnumerator RevealCharacters(TMP_Text textComponent)
    {
        textComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = textComponent.textInfo;
        int totalVisibleCharacters = textInfo.characterCount; // Get total characters
        int visibleCount = 0;

        while (true)
        {
            if (hasTextChanged)
            {
                totalVisibleCharacters = textInfo.characterCount;
                hasTextChanged = false;
            }

            if (visibleCount > totalVisibleCharacters)
            {
                yield return new WaitForSeconds(1.0f);
                visibleCount = 0;
            }

            textComponent.maxVisibleCharacters = visibleCount;
            visibleCount++;

            yield return new WaitForSeconds(revealSpeed); // Delay based on revealSpeed
        }
    }
    IEnumerator RevealWords(TMP_Text textComponent)
    {
        textComponent.ForceMeshUpdate();

        int totalWordCount = textComponent.textInfo.wordCount;
        int totalVisibleCharacters = textComponent.textInfo.characterCount;
        int visibleCount = 0;

        for (int currentWord = 0; currentWord < totalWordCount; currentWord += wordsPerCycle)
        {
            if (hasTextChanged)
            {
                totalVisibleCharacters = textComponent.textInfo.characterCount;
                totalWordCount = textComponent.textInfo.wordCount;
                hasTextChanged = false;
            }

            int lastVisibleCharIndex = textComponent.textInfo.wordInfo[
                Mathf.Min(currentWord + wordsPerCycle - 1, totalWordCount - 1)
            ].lastCharacterIndex;

            visibleCount = lastVisibleCharIndex + 1;

            textComponent.maxVisibleCharacters = visibleCount;

            yield return new WaitForSeconds(revealSpeed);
        }

        // Optionally pause at the end of all words
        yield return new WaitForSeconds(1.0f);
    }

    void OnDisable()
    {
        backToTitleAction.performed -= BackToTitle;
        backToTitleAction.Disable();
    }

}
