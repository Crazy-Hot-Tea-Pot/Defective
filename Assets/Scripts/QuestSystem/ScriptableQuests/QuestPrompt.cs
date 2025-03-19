using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestPrompt", menuName = "Quest/QuestPrompt")]
public class QuestPrompt : Quest
{
    private bool PopUp = true;
    public string PopupText;
    [Tooltip("Option quests if you put nothing here it will not cause a problem")]
    public Quest nextQuest = null;
    public bool InCombat;
    public bool Index;

    public override void RunQuest()
    {
        if (GameManager.Instance.CurrentGameMode == GameManager.GameMode.Combat)
        {
            TriggerPopup();
            PopUp = false;
        }
    }


    public override void TriggerPopup()
    {
        if (PopUp)
        {
            if (Index)
            {
                QuestManager.Instance.CreateConfirmationWindow(PopupText, nextQuest, this);
            }
            else
            {
                QuestManager.Instance.CreateNullConfirmationWindow(PopupText, this);
            }
        }
    }
}
