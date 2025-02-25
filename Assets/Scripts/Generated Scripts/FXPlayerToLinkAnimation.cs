using UnityEngine;

public class FXPlayerToLinkAnimation : MonoBehaviour
{
    // START CUSTOM
    public PlayerController PlayerController;

    public void CustomPlayFootSteps()
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
        //ASK LAY IF TO PUT DELAY, OR AT SAME TIME 
        //If help is less than 35% play another audio
        if (PlayerController.Health <= PlayerController.MaxHealth * 0.35f)
        {
            SoundManager.PlayFXSound(SoundFX.DEFECTION_Glitch);
        }
    }

    // END CUSTOM

    public void PlaySound_AlertSecurityDrone()
    {
        SoundManager.PlayFXSound(SoundFX.AlertSecurityDrone);
    }
    public void PlaySound_AlertSecurityDroneVariantSound()
    {
        SoundManager.PlayFXSound(SoundFX.AlertSecurityDroneVariantSound);
    }
    public void PlaySound_BattleStart()
    {
        SoundManager.PlayFXSound(SoundFX.BattleStart);
    }
    public void PlaySound_Buff()
    {
        SoundManager.PlayFXSound(SoundFX.Buff);
    }
    public void PlaySound_Charging_Up()
    {
        SoundManager.PlayFXSound(SoundFX.Charging_Up);
    }
    public void PlaySound_ChipPlayed()
    {
        SoundManager.PlayFXSound(SoundFX.ChipPlayed);
    }
    public void PlaySound_Click()
    {
        SoundManager.PlayFXSound(SoundFX.Click);
    }
    public void PlaySound_Click_Move()
    {
        SoundManager.PlayFXSound(SoundFX.Click_Move);
    }
    public void PlaySound_Click_Move1()
    {
        SoundManager.PlayFXSound(SoundFX.Click_Move1);
    }
    public void PlaySound_Click_Move2()
    {
        SoundManager.PlayFXSound(SoundFX.Click_Move2);
    }
    public void PlaySound_DamageTaken()
    {
        SoundManager.PlayFXSound(SoundFX.DamageTaken);
    }
    public void PlaySound_Debuff()
    {
        SoundManager.PlayFXSound(SoundFX.Debuff);
    }
    public void PlaySound_DisassembleMaintenanceBot()
    {
        SoundManager.PlayFXSound(SoundFX.DisassembleMaintenanceBot);
    }
    public void PlaySound_DrawingOutTheDeck()
    {
        SoundManager.PlayFXSound(SoundFX.DrawingOutTheDeck);
    }
    public void PlaySound_EnemyDefeated()
    {
        SoundManager.PlayFXSound(SoundFX.EnemyDefeated);
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
    public void PlaySound_FootstepLoop()
    {
        SoundManager.PlayFXSound(SoundFX.FootstepLoop);
    }
    public void PlaySound_GalvanizeMainenanceBot()
    {
        SoundManager.PlayFXSound(SoundFX.GalvanizeMainenanceBot);
    }
    public void PlaySound_MenuSelectClick()
    {
        SoundManager.PlayFXSound(SoundFX.MenuSelectClick);
    }
    public void PlaySound_MenuSelectClick1()
    {
        SoundManager.PlayFXSound(SoundFX.MenuSelectClick1);
    }
    public void PlaySound_MenuSelectionSound()
    {
        SoundManager.PlayFXSound(SoundFX.MenuSelectionSound);
    }
    public void PlaySound_NeutralizeSecurityDrone()
    {
        SoundManager.PlayFXSound(SoundFX.NeutralizeSecurityDrone);
    }
    public void PlaySound_NeutralizeSecurityDroneAlternative()
    {
        SoundManager.PlayFXSound(SoundFX.NeutralizeSecurityDroneAlternative);
    }
    public void PlaySound_Punch()
    {
        SoundManager.PlayFXSound(SoundFX.Punch);
    }
    public void PlaySound_RepairMaintenaceBot()
    {
        SoundManager.PlayFXSound(SoundFX.RepairMaintenaceBot);
    }
    public void PlaySound_Running()
    {
        SoundManager.PlayFXSound(SoundFX.Running);
    }
    public void PlaySound_ShredGarbageBot()
    {
        SoundManager.PlayFXSound(SoundFX.ShredGarbageBot);
    }
}
