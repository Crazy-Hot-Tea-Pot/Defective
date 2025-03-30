using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static SoundAsset;

public static class SoundManager
{
    private static Dictionary<SoundFX, AudioSource> loopingSounds = new Dictionary<SoundFX, AudioSource>();

    /// <summary>
    /// Change background sound.
    /// </summary>
    /// <param name="bgSound">New sound</param>
    public static void ChangeBackground(BgSound bgSound)
    {
        if (bgSound == BgSound.None)
        {
            Debug.Log("No sound selected. Skipping background sound change.");
            return;
        }


        if (SettingsManager.Instance.SoundSettings.BGMMute)
            return;

        //Delete old sound
        GameObject.Destroy(GameObject.Find("BgSound"));


        StartBackgroundSound(bgSound);
    }

    /// <summary>
    /// Change pitch of background sound
    /// </summary>
    /// <param name="Pitch">Is a float between 0 and 1.</param>
    public static void ChangeBackgroundPitch(float Pitch)
    {
        GameObject.Find("BgSound").GetComponent<AudioSource>().pitch = Pitch;
    }

    public static void StartPersistentBackgroundSound(BgSound bgSound)
    {
        // Check if a persistent background sound GameObject already exists
        GameObject existingBGSound = GameObject.Find("BgSound");
        if (existingBGSound != null)
        {
            // Change the clip if a different background sound is requested
            AudioSource existingAudioSource = existingBGSound.GetComponent<AudioSource>();
            if (existingAudioSource.clip != GetBGAudio(bgSound))
            {
                existingAudioSource.clip = GetBGAudio(bgSound);
                existingAudioSource.Play();
            }
            return;
        }

        // Create a new GameObject for the persistent background music
        GameObject persistentBGSound = new GameObject("BgSound");

        // Add an AudioSource component to play the audio
        AudioSource audioSource = persistentBGSound.AddComponent<AudioSource>();
        audioSource.clip = GetBGAudio(bgSound);
        audioSource.loop = true;
        audioSource.volume = SettingsManager.Instance.SoundSettings.GetBGSoundForComponent();
        audioSource.Play();

        // Prevent this GameObject from being destroyed on scene load
        Object.DontDestroyOnLoad(persistentBGSound);

        // Add the SoundBGVolume component for volume control if needed
        persistentBGSound.AddComponent<SoundBGVolume>();
    }

    /// <summary>
    /// Starts playing background music on a loop.
    /// Volume can be adjusted using the SoundBGVolume component.
    /// </summary>
    /// <param name="bgSound">The background sound to play.</param>
    public static void StartBackgroundSound(BgSound bgSound)
    {
        if (bgSound == BgSound.None)
        {
            Debug.Log("No sound selected. Skipping background sound change.");
            return;
        }

        //if mute don't bother to spawn sound
        if (SettingsManager.Instance.SoundSettings.BGMMute)
        {
            return;
        }
        // Create a new GameObject for background music
        GameObject BackgroundSound = new GameObject("BgSound");

        // Add an AudioSource component to play the audio
        AudioSource audioSource = BackgroundSound.AddComponent<AudioSource>();
        // Assign background audio clip
        audioSource.clip = GetBGAudio(bgSound);
        // Set looping for continuous play
        audioSource.loop = true;
        // Set initial volume
        audioSource.volume = SettingsManager.Instance.SoundSettings.GetBGSoundForComponent();
        audioSource.Play();
        BackgroundSound.AddComponent<SoundBGVolume>();
    }
    /// <summary>
    /// Plays a sound effect for a single iteration with a 2D sound effect.
    /// </summary>
    /// <param name="sound">The specific sound effect to play.</param>
    public static void PlayFXSound(SoundFX sound)
    {
        if (sound == SoundFX.None)
        {
            Debug.Log("No sound selected. Skipping sound effect.");
            return;
        }
        //if mute don't bother to spawn sound
        if (SettingsManager.Instance.SoundSettings.SFXMute)
        {
            return;
        }

        // Create a new GameObject for this sound effect
        GameObject soundGameObject = new GameObject("SoundFX");

        // Add an AudioSource component to handle playback
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        // Set the lifetime of the sound effect so it is destroyed after playback
        soundGameObject.AddComponent<SoundFXLife>().SoundLength = GetAudio(sound).length;

        // Configure the volume of the sound effect based on settings
        audioSource.volume = SettingsManager.Instance.SoundSettings.GetSFXSoundForComponent();
        audioSource.PlayOneShot(GetAudio(sound));                        
    }
    /// <summary>
    /// Same as regular play FX Sound but for 3D effect.
    /// </summary>
    /// <param name="sound">Enum of sound</param>
    /// <param name="soundOrigin">Where you want the sound to come from.</param>
    public static void PlayFXSound(SoundFX sound, Transform soundOrigin)
    {
        if (sound == SoundFX.None)
        {
            Debug.Log("No sound selected. Skipping sound effect.");
            return;
        }

        if (soundOrigin == null)
        {
            Debug.LogError("Sound origin not provided for 3D sound. Skipping sound effect.");
            return;
        }

        // PlayClipAtPoint directly (No need to create GameObject or use PlayOneShot)
        AudioSource.PlayClipAtPoint(GetAudio(sound), soundOrigin.position);

    }
    /// <summary>
    /// Plays a sound effect with options to loop and choose between 2D or 3D sound.
    /// </summary>
    /// <param name="sound">The sound effect to play.</param>
    /// <param name="loop">Set to true to loop the sound effect.</param>
    /// <param name="is3D">Set to true to play the sound as a 3D sound.</param>
    /// <param name="soundOrigin">Where you want the sound to come from.</param>
    public static void PlayFXSound(SoundFX sound, bool loop, bool is3D,Transform soundOrigin=null)
    {
        if (sound == SoundFX.None || SettingsManager.Instance.SoundSettings.SFXMute)
        {
            Debug.Log("No sound selected or SFX muted. Skipping sound.");
            return;
        }

        // Handle looping sounds
        if (loop)
        {
            if (loopingSounds.ContainsKey(sound))
            {
                Debug.Log($"Sound {sound} is already playing in a loop.");
                return;
            }

            GameObject soundGameObject = new GameObject($"LoopingSoundFX_{sound}");
            if (soundOrigin != null)
            {
                soundGameObject.transform.position = soundOrigin.position;
                soundGameObject.transform.SetParent(soundOrigin);
            }

            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            audioSource.clip = GetAudio(sound);
            audioSource.volume = SettingsManager.Instance.SoundSettings.GetSFXSoundForComponent();
            audioSource.loop = true;

            if (is3D)
            {
                // Fully 3D
                audioSource.spatialBlend = 1.0f;
            }
            else
            {
                // Fully 2D
                audioSource.spatialBlend = 0.0f;
            }

            audioSource.Play();
            loopingSounds.Add(sound, audioSource);
            Debug.Log($"Started looping sound: {sound}");
        }
        else
        {
            GameObject soundGameObject = new GameObject("SoundFX");

            Vector3 soundPosition = soundOrigin != null ? soundOrigin.position : Vector3.zero;
            soundGameObject.transform.position = soundPosition;

            if (soundOrigin != null)
            {
                soundGameObject.transform.SetParent(soundOrigin);
            }


            AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
            soundGameObject.AddComponent<SoundFXLife>().SoundLength = GetAudio(sound).length;
            audioSource.volume = SettingsManager.Instance.SoundSettings.GetSFXSoundForComponent();

            if (is3D)
            {
                // 3D sound
                if (soundOrigin != null)
                {
                    AudioSource.PlayClipAtPoint(GetAudio(sound), soundOrigin.position);
                }
                else
                {
                    Debug.LogWarning($"Sound {sound} set to 3D but no soundOrigin provided. Playing at (0,0,0).");
                    AudioSource.PlayClipAtPoint(GetAudio(sound), Vector3.zero);
                }
            }
            else
            {
                // 2D sound
                audioSource.spatialBlend = 0.0f;
                audioSource.PlayOneShot(GetAudio(sound));
            }
        }
    }
    /// <summary>
    /// Stops a looping sound effect.
    /// </summary>
    /// <param name="sound">The sound effect to stop.</param>
    public static void StopLoopingFXSound(SoundFX sound)
    {
        if (loopingSounds.ContainsKey(sound))
        {
            AudioSource audioSource = loopingSounds[sound];
            if (audioSource != null)
            {
                audioSource.Stop();
                GameObject.Destroy(audioSource.gameObject);
            }
            loopingSounds.Remove(sound);
            Debug.Log($"Looping sound {sound} stopped.");
        }
        else
        {
            Debug.Log($"No looping sound found for {sound}.");
        }
    }
    /// <summary>
    /// Gets the current Bg Sound playing.
    /// </summary>
    /// <returns></returns>
    public static BgSound GetCurrentBackgroundSound()
    {
        GameObject bgSoundObject = GameObject.Find("BgSound");
        if (bgSoundObject != null)
        {
            AudioSource audioSource = bgSoundObject.GetComponent<AudioSource>();
            BgSound? bgSound = SoundAsset.soundAssets.soundBGArray
                .FirstOrDefault(bg => bg.audioClip == audioSource.clip)?.bgSound;

            return bgSound ?? BgSound.Background; 
        }
        return BgSound.Background;
    }

    /// <summary>
    /// Gets clip of the sfx thats about to be played
    /// </summary>
    /// <param name="sound"></param>
    /// <returns></returns>
    public static AudioClip GetSoundFxClip(SoundFX sound)
    {
        return SoundAsset.soundAssets.soundFXClipArray
            .FirstOrDefault(s => s.soundFX == sound)?.audioClip;
    }


    private static AudioClip GetBGAudio(BgSound sound)
    {
        foreach (SoundAsset.BgSounds Bgsound in SoundAsset.soundAssets.soundBGArray)
        {
            if (Bgsound.bgSound == sound)
            {
                return Bgsound.audioClip;
            }
        }
        Debug.LogError("Sound" + sound + "not found");
        return null;
    }
    /// <summary>
    /// Gets audio clip from sound assets
    /// </summary>
    /// <param name="sound"></param>
    /// <returns></returns>
    private static AudioClip GetAudio(SoundFX sound)
    {
        foreach (SoundAsset.SoundFXClip soundFxClip in SoundAsset.soundAssets.soundFXClipArray)
        {
            if (soundFxClip.soundFX == sound)
            {
                return soundFxClip.audioClip;
            }
        }
        Debug.LogError("Sound" + sound + "not found");
        return null;
    }
}
