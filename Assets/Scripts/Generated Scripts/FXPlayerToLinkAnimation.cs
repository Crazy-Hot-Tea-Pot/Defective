using UnityEngine;

public class FXPlayerToLinkAnimation : MonoBehaviour
{
    // START CUSTOM
    public PlayerController PlayerController;

    public void CustomPlayFootSteps()
    {
        //If help is less than 35% play another audio
        if (PlayerController.Health <= PlayerController.MaxHealth * 0.35f)
        {
            // Randomly select a footstep sound
            int randomIndex = Random.Range(1, 6);
            switch (randomIndex)
            {
                case 1:
                    PlaySound_FootStepGlitch();
                    break;
                case 2:
                    PlaySound_FootStepGlitch1();
                    break;
                case 3:
                    PlaySound_FootStepGlitch2();
                    break;
                case 4:
                    PlaySound_FootStepGlitch3();
                    break;
                case 5:
                    PlaySound_FootStepGlitch4();
                    break;
                case 6:
                    PlaySound_FootStepGlitch5();
                    break;
            }
        }
        else
        {
            // Randomly select a footstep sound
            int randomIndex = Random.Range(1, 6);
            switch (randomIndex)
            {
                case 1:
                    PlaySound_Footstep();
                    break;
                case 2:
                    PlaySound_Footstep1();
                    break;
                case 3:
                    PlaySound_Footstep2();
                    break;
                case 4:
                    PlaySound_Footstep3();
                    break;
                case 5:
                    PlaySound_Footstep4();
                    break;
                case 6:
                    PlaySound_Footstep5();
                    break;
            }
        }
    }

    // END CUSTOM
    public void PlaySound_BattleWin()
    {
        SoundManager.PlayFXSound(SoundFX.BattleWin);
    }
    public void PlaySound_ChipsPlay()
    {
        SoundManager.PlayFXSound(SoundFX.ChipsPlay);
    }
    public void PlaySound_DeckClose()
    {
        SoundManager.PlayFXSound(SoundFX.DeckClose);
    }
    public void PlaySound_DeckOpen()
    {
        SoundManager.PlayFXSound(SoundFX.DeckOpen);
    }
    public void PlaySound_Drained()
    {
        SoundManager.PlayFXSound(SoundFX.Drained);
    }
    public void PlaySound_DroneAlarm()
    {
        SoundManager.PlayFXSound(SoundFX.DroneAlarm);
    }
    public void PlaySound_DroneLaser()
    {
        SoundManager.PlayFXSound(SoundFX.DroneLaser);
    }
    public void PlaySound_DroneRam()
    {
        SoundManager.PlayFXSound(SoundFX.DroneRam);
    }
    public void PlaySound_EnterDoorSfx()
    {
        SoundManager.PlayFXSound(SoundFX.EnterDoorSfx);
    }
    public void PlaySound_Footstep()
    {
        SoundManager.PlayFXSound(SoundFX.Footstep);
    }
    public void PlaySound_Footstep1()
    {
        SoundManager.PlayFXSound(SoundFX.Footstep1);
    }
    public void PlaySound_Footstep2()
    {
        SoundManager.PlayFXSound(SoundFX.Footstep2);
    }
    public void PlaySound_Footstep3()
    {
        SoundManager.PlayFXSound(SoundFX.Footstep3);
    }
    public void PlaySound_Footstep4()
    {
        SoundManager.PlayFXSound(SoundFX.Footstep4);
    }
    public void PlaySound_Footstep5()
    {
        SoundManager.PlayFXSound(SoundFX.Footstep5);
    }
    public void PlaySound_FootStepGlitch()
    {
        SoundManager.PlayFXSound(SoundFX.FootStepGlitch);
    }
    public void PlaySound_FootStepGlitch1()
    {
        SoundManager.PlayFXSound(SoundFX.FootStepGlitch1);
    }
    public void PlaySound_FootStepGlitch2()
    {
        SoundManager.PlayFXSound(SoundFX.FootStepGlitch2);
    }
    public void PlaySound_FootStepGlitch3()
    {
        SoundManager.PlayFXSound(SoundFX.FootStepGlitch3);
    }
    public void PlaySound_FootStepGlitch4()
    {
        SoundManager.PlayFXSound(SoundFX.FootStepGlitch4);
    }
    public void PlaySound_FootStepGlitch5()
    {
        SoundManager.PlayFXSound(SoundFX.FootStepGlitch5);
    }
    public void PlaySound_Galvanize()
    {
        SoundManager.PlayFXSound(SoundFX.Galvanize);
    }
    public void PlaySound_GameOver()
    {
        SoundManager.PlayFXSound(SoundFX.GameOver);
    }
    public void PlaySound_GuardSound()
    {
        SoundManager.PlayFXSound(SoundFX.GuardSound);
    }
    public void PlaySound_Jam()
    {
        SoundManager.PlayFXSound(SoundFX.Jam);
    }
    public void PlaySound_LaserBeam()
    {
        SoundManager.PlayFXSound(SoundFX.LaserBeam);
    }
    public void PlaySound_LuckyTrinket()
    {
        SoundManager.PlayFXSound(SoundFX.LuckyTrinket);
    }
    public void PlaySound_MenuSelectClick()
    {
        SoundManager.PlayFXSound(SoundFX.MenuSelectClick);
    }
    public void PlaySound_MenuSelectionSound()
    {
        SoundManager.PlayFXSound(SoundFX.MenuSelectionSound);
    }
    public void PlaySound_Powerboost()
    {
        SoundManager.PlayFXSound(SoundFX.Powerboost);
    }
    public void PlaySound_RobotPunchHitHuman()
    {
        SoundManager.PlayFXSound(SoundFX.RobotPunchHitHuman);
    }
    public void PlaySound_RobotPunchMetal()
    {
        SoundManager.PlayFXSound(SoundFX.RobotPunchMetal);
    }
    public void PlaySound_ShivHitHuman()
    {
        SoundManager.PlayFXSound(SoundFX.ShivHitHuman);
    }
    public void PlaySound_ShivHitMetal()
    {
        SoundManager.PlayFXSound(SoundFX.ShivHitMetal);
    }
    public void PlaySound_SteelPlatingSound()
    {
        SoundManager.PlayFXSound(SoundFX.SteelPlatingSound);
    }
    public void PlaySound_WornDown()
    {
        SoundManager.PlayFXSound(SoundFX.WornDown);
    }
}
