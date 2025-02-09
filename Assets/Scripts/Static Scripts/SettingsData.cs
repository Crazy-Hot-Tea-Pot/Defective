using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A constructor for the settings menu in the ui for saving it's various components
/// </summary>
[System.Serializable]
public class SettingsData
{
    [System.Serializable]
    public class WriteVideoFileData
    {
        public bool SettingsEdited;
        public int resolution;

        public float gainX, gainY, gainZ, gainW;

        public float gammaX,gammaY, gammaZ, gammaW;

        public bool bloom;

        public bool windowedMode;

        /// <summary>
        /// Convenience method to get Vector4 Gain
        /// </summary>
        public Vector4 Gain
        {
            get => new Vector4(gainX, gainY, gainZ, gainW);
            set
            {
                gainX = value.x;
                gainY = value.y;
                gainZ = value.z;
                gainW = value.w;
            }
        }
        public Vector4 Gamma
        {
            get => new Vector4(gammaX, gammaY, gammaZ, gammaW);
            set
            {
                gammaX = value.x;
                gammaY = value.y;
                gammaZ = value.z;
                gammaW = value.w;
            }
        }
    }

    [System.Serializable]
    public class CameraSettingsData
    {
        public bool SettingsEdited;
        public float CameraSpeed;
        public bool BoarderMouseMovement;
    }

    [System.Serializable]
    public class DataSettingsData
    {
        public bool SettingsEdited;
        public int MaxAutoSaves;
    }

    [System.Serializable]
    public class SoundSettingsData
    {
        public bool SettingsEdited;
        public bool BGMMute;
        public bool SFXMute;
        public float BGMVolume;
        public float SFXVolume;
    }

    public WriteVideoFileData VideoData;
    public CameraSettingsData DataForCameraSettings;
    public DataSettingsData DataForDataSettings;
    public SoundSettingsData DataForSoundSettings;

    public SettingsData()
    {
        VideoData = new WriteVideoFileData();
        DataForCameraSettings = new CameraSettingsData();
        DataForDataSettings = new DataSettingsData();
        DataForSoundSettings = new SoundSettingsData();
    }
}
