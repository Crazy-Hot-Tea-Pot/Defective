using System;
using UnityEngine;
using static SettingsData;

[System.Serializable]
public class SoundSettings
{
    //Public even for when BGM volume has change for any BGM being played.
    public event System.Action<float> OnBGMVolumeChanged;

    /// <summary>
    /// BG volume.
    /// Do not use this value to apply to components in game.
    /// use GetBGSoundForComponentsMethod
    /// </summary>
    public float BGMVolume
    {
        get
        {
            return bgmVolume;
        }
        set
        {
            bgmVolume = value;
            OnBGMVolumeChanged?.Invoke(GetBGSoundForComponent());
        }
    }

    /// <summary>
    /// Sound Effect Volume.
    /// Do not use this value to apply to components in game.
    /// use GetSFXSoundForComponent() instead.
    /// </summary>
    public float SFXVolume
    {
        get
        {
            return sfxVolume;
        }
        set
        {
            sfxVolume = value;
        }
    }

    public bool BGMMute
    {
        get
        {
            return bgmMute;
        }
        set
        {
            bgmMute = value;
        }
    }

    public bool SFXMute
    {
        get
        {
            return sfxMute;
        }
        set
        {
            sfxMute = value;
        }
    }

    [SerializeField]
    [Range(0f, 100f)]
    private float bgmVolume;
    [SerializeField]
    [Range(0f,100f)]
    private float sfxVolume;
    [SerializeField]
    private bool bgmMute;
    [SerializeField]
    private bool sfxMute;

    public SoundSettings(SoundSettingsData data)
    {
        if (data.SettingsEdited)
        {
            SFXVolume = data.SFXVolume;
            bgmVolume = data.BGMVolume;
            SFXMute = data.SFXMute;
            BGMMute = data.BGMMute;
        }
        else
        {
            SFXVolume = 100f;
            BGMVolume = 100f;
            SFXMute = false;
            BGMMute = false;
        }
    }

    public float GetSFXSoundForComponent()
    {
        return SFXVolume / 100f;
    }
    public float GetBGSoundForComponent()
    {
        return BGMVolume / 100f;
    }

    public SoundSettingsData GetDataToWrite()
    {
        return new SoundSettingsData
        {
            SettingsEdited = true,
            BGMVolume = BGMVolume,
            SFXVolume = SFXVolume,
        };
    }
}