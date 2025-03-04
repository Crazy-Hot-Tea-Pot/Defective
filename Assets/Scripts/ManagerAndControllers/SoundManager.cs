using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SoundManager
{    

    /// <summary>
    /// Change background sound.
    /// </summary>
    /// <param name="bgSound">New sound</param>
    public static void ChangeBackground(BgSound bgSound)
    {
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
    /// <param name="sound"></param>
    /// <param name="parent"></param>
    public static void PlayFXSound(SoundFX sound, Transform parent)
    {
        GameObject soundGameObject = new GameObject("SoundFX");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        soundGameObject.AddComponent<SoundFXLife>().SoundLength = GetAudio(sound).length;

        //Set audio to 3D
        // Set audio to fully 3D for spatial effects in the scene
        audioSource.spatialBlend = 1.0f;

        // Configure the volume of the sound effect based on settings
        audioSource.volume = SettingsManager.Instance.SoundSettings.GetSFXSoundForComponent();

        // Play the sound once using PlayOneShot, so other sounds are not interrupted
        audioSource.PlayOneShot(GetAudio(sound));

        //Set Parent
        //soundGameObject.transform.parent = parent;
        // Reset position relative
        //soundGameObject.transform.localPosition = Vector3.zero;

        //trying this
        AudioSource.PlayClipAtPoint(GetAudio(sound),parent.position);                
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
