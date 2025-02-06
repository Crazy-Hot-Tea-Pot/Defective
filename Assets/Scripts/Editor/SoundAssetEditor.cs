using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(SoundAsset))]
public class SoundAssetEditor : Editor
{
    private int selectedBGIndex = 0;
    private int selectedFXIndex = 0;
    private static AudioSource previewAudioSource;

    void OnEnable()
    {
        // Automatically regenerate enums
        SoundEnumGenerator.GenerateEnums(); 

        SoundAsset soundAsset = (SoundAsset)target;

        //Automatically Populate
        //PopulateSoundArrays(soundAsset);

        // Create an AudioSource for previewing sounds if it doesn't exist
        if (previewAudioSource == null)
        {
            GameObject previewObject = new GameObject("AudioPreviewer");
            previewObject.hideFlags = HideFlags.HideAndDontSave; // Ensure it's hidden and persistent
            previewAudioSource = previewObject.AddComponent<AudioSource>();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SoundAsset soundAsset = (SoundAsset)target;

        // Background Sound Preview
        EditorGUILayout.LabelField("Background Sounds", EditorStyles.boldLabel);
        if (soundAsset.soundBGArray != null && soundAsset.soundBGArray.Length > 0)
        {
            string[] bgNames = System.Array.ConvertAll(soundAsset.soundBGArray, s => s.bgSound.ToString());
            selectedBGIndex = EditorGUILayout.Popup("Select BG Sound", selectedBGIndex, bgNames);

            if (GUILayout.Button("Play Selected BG Sound"))
            {
                PlaySound(soundAsset.soundBGArray[selectedBGIndex].audioClip);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No Background Sounds Available", MessageType.Warning);
        }

        // Sound FX Preview
        EditorGUILayout.LabelField("Sound FX", EditorStyles.boldLabel);
        if (soundAsset.soundFXClipArray != null && soundAsset.soundFXClipArray.Length > 0)
        {
            string[] fxNames = System.Array.ConvertAll(soundAsset.soundFXClipArray, s => s.soundFX.ToString());
            selectedFXIndex = EditorGUILayout.Popup("Select FX Sound", selectedFXIndex, fxNames);

            if (GUILayout.Button("Play Selected FX Sound"))
            {
                PlaySound(soundAsset.soundFXClipArray[selectedFXIndex].audioClip);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No Sound FX Available", MessageType.Warning);
        }

        // Manual Update EquipButton
        if (GUILayout.Button("Manually Update Sound Assets"))
        {
            PopulateSoundArrays(soundAsset);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (previewAudioSource != null && clip != null)
        {
            previewAudioSource.clip = clip;
            previewAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("No audio source available for preview or audio clip is null.");
        }
    }    

    /// <summary>
    /// Gets sounds from resource folders and generates a script with enums.
    /// This is for the SoundManager to use.
    /// </summary>
    /// <param name="soundAsset"></param>
    private void PopulateSoundArrays(SoundAsset soundAsset)
    {
        // Load Background Sounds from Resources/Sounds/BG
        AudioClip[] bgClips = Resources.LoadAll<AudioClip>("Sounds/BG");
        soundAsset.soundBGArray = new SoundAsset.BgSounds[bgClips.Length];
        for (int i = 0; i < bgClips.Length; i++)
        {
            string clipName = bgClips[i].name.Replace(" ", "_");
            if (System.Enum.TryParse(clipName, out BgSound bgEnumValue))
            {
                soundAsset.soundBGArray[i] = new SoundAsset.BgSounds
                {
                    bgSound = bgEnumValue,
                    audioClip = bgClips[i]
                };
            }
            else
            {
                Debug.LogWarning($"Enum not found for BG sound: {clipName}");
            }
        }

        // Load Sound Effects from Resources/Sounds/FX
        AudioClip[] fxClips = Resources.LoadAll<AudioClip>("Sounds/FX");
        soundAsset.soundFXClipArray = new SoundAsset.SoundFXClip[fxClips.Length];
        for (int i = 0; i < fxClips.Length; i++)
        {
            string clipName = fxClips[i].name.Replace(" ", "_");
            if (System.Enum.TryParse(clipName, out SoundFX fxEnumValue))
            {
                soundAsset.soundFXClipArray[i] = new SoundAsset.SoundFXClip
                {
                    soundFX = fxEnumValue,
                    audioClip = fxClips[i]
                };
            }
            else
            {
                Debug.LogWarning($"Enum not found for FX sound: {clipName}");
            }
        }

        // Save changes to the prefab instance
        EditorUtility.SetDirty(soundAsset);
        Debug.Log("Sound arrays populated successfully.");
    }


}
